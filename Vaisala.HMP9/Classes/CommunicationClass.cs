using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Vaisala.HMP.Enums;
using Vaisala.HMP.Models;
using Vaisala.HMP.Models.Messages;

namespace Vaisala.HMP.Classes
{
    public class CommunicationClass
    {
        private readonly SerialPort _port;
        private bool _reverseOrderFloat;

        #region Public properties

        public byte DeviceId { get; private set; }

        #endregion

        #region Public methods
        public string[] DiscoverConnectedSerialPorts() => SerialPort.GetPortNames();
        #endregion

        #region Constructors
        public CommunicationClass(byte deviceId) {
            DeviceId = deviceId;
            Task.Run(async () => await TestCommunication()).Wait();
        }

        public CommunicationClass(string portName, byte deviceId) {
            _port = new SerialPort(portName: portName, baudRate: 19200, parity: Parity.None, dataBits: 8, stopBits: StopBits.Two);
            DeviceId = deviceId;
            Task.Run(async () => await TestCommunication()).Wait();
        }

        public CommunicationClass(string portName, byte deviceId, int baudRate, Parity parity, int dataBits, StopBits stopBits) {
            _port = new SerialPort(portName: portName, baudRate: baudRate, parity: parity, dataBits: dataBits, stopBits: stopBits);
            DeviceId = deviceId;
            Task.Run(async () => await TestCommunication()).Wait();
        }

        #endregion

        private async Task TestCommunication() {

            Task openPortTask = Task.Run(() => _port.Open());

            await Task.Delay(5000);
            await Task.WhenAny(openPortTask, Task.Delay(2000));

            if (!_port.IsOpen)
                return;

            _port.NewLine = "\r\n";
            _port.RtsEnable = true;
            _port.BaseStream.WriteTimeout = 1000;
            _port.BaseStream.ReadTimeout = 1000;

            var test1Response = await SendRequest(TestMessages.SignedIntegerTest);
            var test2Response = await SendRequest(TestMessages.FloatingPointTest);
            var test3Response = await SendRequest(TestMessages.TextStringTest);

            DeviceTest test = new DeviceTest(test1: test1Response, test2: test2Response, test3: test3Response);
            _reverseOrderFloat = test.ReverseOrderFloat;
        }

        public async Task<float> GetValue(RequestStruct request) {
            var response = await SendRequest(request);
            return response.GetNumeric<float>();
        }

        public async Task<DeviceIdentificationModel[]> GetValue(RequestExtendedStruct request) => (await SendRequest(request.ToBytes(DeviceId, false))).DeviceIdentification;

        private async Task<ModbusMessageStruct[]> SendRequest(RequestStruct message) => (await SendRequest(message.ToBytes(DeviceId, false))).HoldingRegisters;

        private async Task<ResponseModel> SendRequest(byte[] buffer) {
            try {

                await _port.BaseStream.FlushAsync();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();

                await _port.BaseStream.WriteAsync(buffer: buffer, offset: 0, count: buffer.Length);

                return await ReadResponse();
            }
            catch (Exception exception) {
                Console.WriteLine(exception.StackTrace);
                return default;
            }
        }

        private async Task<ResponseModel> ReadResponse() {
            if (!_port.IsOpen)
                return new ResponseModel();

            var (deviceValid, deviceValues) = await ReadByte();
            if (!deviceValid)
                return new ResponseModel();

            var (functionValid, functionCodeId) = await ReadByte();
            if (!functionValid)
                return new ResponseModel();

            byte deviceId = deviceValues;
            FunctionType functionCode = (FunctionType)functionCodeId;

            switch (functionCode) {
                case FunctionType.ReadHoldingRegisters:
                    var (expectedByteValid, expectedByteCount) = await ReadByte();
                    if (!expectedByteValid)
                        return new ResponseModel();

                    return await ReadSimpleMessage(deviceId: deviceId, functionCode: functionCodeId, expectedBytes: expectedByteCount);

                case FunctionType.WriteMultipleRegisters:
                    //expectedBytes = 4;
                    break;
                case FunctionType.ReadDeviceIdentification:
                    return await ReadDeviceInfo(deviceId: deviceId, functionCode: functionCodeId, subHeadLength: 6);
                default:
                    return new ResponseModel();
            };


            return new ResponseModel();
        }

        private async Task<ResponseModel> ReadSimpleMessage(byte deviceId, byte functionCode, byte expectedBytes) {
            
            var (responseValid, bodyMessage) = await ReadBytes((uint)(expectedBytes + MessageTranslatorExtension.Crc16Length));
            if (!responseValid)
                return new ResponseModel();

            byte[] fullMessage = new byte[MessageTranslatorExtension.ResponseHeadLength + MessageTranslatorExtension.Crc16Length + expectedBytes];
            fullMessage[0] = deviceId;
            fullMessage[1] = functionCode;
            fullMessage[2] = expectedBytes;

            Array.Copy(bodyMessage, 0, fullMessage, MessageTranslatorExtension.ResponseHeadLength, bodyMessage.Length);

            bool messageValid = fullMessage.CheckResponseMessage();

            if (!messageValid)
                return new ResponseModel();

            ModbusMessageStruct[] modbusReponse = new ModbusMessageStruct[expectedBytes / 2];

            for (int i = 0; i < modbusReponse.Length; i++)
                modbusReponse[i] = new ModbusMessageStruct(bodyMessage[(i * 2)..(i * 2 + 2)]);

            return new ResponseModel(modbusReponse);
        }

        private async Task<ResponseModel> ReadDeviceInfo(byte deviceId, byte functionCode, byte subHeadLength) {
            
            var (validHeader, subHeader) = await ReadBytes(subHeadLength);
            if (!validHeader)
                return new ResponseModel();

            byte[][] messages = new byte[subHeader[^1]][];
            int totalSize = 0;
            for (int i = 0; i < messages.Length; i++) {
                var (validId, id) = await ReadByte();
                if (!validId)
                    return new ResponseModel();

                var (validLength, length) = await ReadByte();
                if (!validLength)
                    return new ResponseModel();

                var (validMessage, message) = await ReadBytes(length);
                if (!validMessage)
                    return new ResponseModel();

                messages[i] = new byte[2 + length];
                messages[i][0] = id;
                messages[i][1] = length;

                Array.Copy(message, 0, messages[i], 2, length);

                totalSize += 2 + length;
            }

            byte[] fullMessage = new byte[MessageTranslatorExtension.ResponseHeadDeviceInformationLength + MessageTranslatorExtension.ResponseSubHeadLength + MessageTranslatorExtension.Crc16Length + totalSize];
            fullMessage[0] = deviceId;
            fullMessage[1] = functionCode;

            Array.Copy(subHeader, 0, fullMessage, 2, MessageTranslatorExtension.ResponseSubHeadLength);

            int offset = (int)(MessageTranslatorExtension.ResponseHeadDeviceInformationLength + MessageTranslatorExtension.ResponseSubHeadLength); ;
            for (int i = 0; i < messages.Length; i++) {
                Array.Copy(messages[i], 0, fullMessage, offset, messages[i].Length);
                offset += messages[i].Length;
            }

            var (validCrc16, crc16) = await ReadBytes(MessageTranslatorExtension.Crc16Length);
            if (!validCrc16)
                return new ResponseModel();
            
            Array.Copy(crc16, 0, fullMessage, fullMessage.Length - MessageTranslatorExtension.Crc16Length, MessageTranslatorExtension.Crc16Length);

            bool crc16Confirmed = fullMessage.CheckResponseMessage();
            if (!crc16Confirmed)
                return new ResponseModel("CRC16 Checksum failed");

            DeviceIdentificationModel[] deviceIdentifications = new DeviceIdentificationModel[messages.Length];
            for (int i = 0; i < messages.Length; i++)
                deviceIdentifications[i] = messages[i].ToDeviceIdentificationModel();

            return new ResponseModel(deviceIdentifications);
        }

        private async Task<(bool valid, byte value)> ReadByte() {
            var (valid, values) =  await ReadBytes(1);
            if (valid)
                return (valid, values[0]);
            else
                return (valid, default);
        }

        private async Task<(bool valid, byte[] values)> ReadBytes(uint length) {
            byte[] buffer = new byte[length];
            int count = await _port.BaseStream.ReadAsync(buffer: buffer, offset: 0, count: buffer.Length);

            if (count < 1)
                return (false, new byte[0]);

            return (true, buffer);
        }
    }

}
