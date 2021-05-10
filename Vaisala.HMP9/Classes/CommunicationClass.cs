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

        public async Task<float> GetValue(RequestStruct message) {
            var response = await SendRequest(message);
            return response.GetNumeric<float>();
        }

        private async Task<ModbusMessageStruct[]> SendRequest(RequestStruct message) {
            try {

                await _port.BaseStream.FlushAsync();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();

                byte[] buffer = message.ToBytes(DeviceId, false);
                await _port.BaseStream.WriteAsync(buffer: buffer, offset: 0, count: buffer.Length);

                var response = await ReadResponse();

                if (!response.valid)
                    return default;

                return response.response;
            }
            catch (Exception exception) {
                Console.WriteLine(exception.StackTrace);
                return default;
            }
        }

        private async Task<(bool valid, ModbusMessageStruct[] response)> ReadResponse() {
            if (!_port.IsOpen)
                return (false, default);

            var (deviceValid, deviceValues) = await ReadByte();
            if (!deviceValid)
                return (false, default);

            var (functionValid, functionCodeId) = await ReadByte();
            if (!functionValid)
                return (false, default);

            byte deviceId = deviceValues;
            FunctionCode functionCode = (FunctionCode)functionCodeId;

            byte expectedBytes;
            switch (functionCode) {
                case FunctionCode.ReadHoldingRegisters:
                    var (expectedByteValid, expectedByteCount) = await ReadByte();
                    if (!expectedByteValid)
                        return (false, default);

                    expectedBytes = expectedByteCount;
                    break;
                case FunctionCode.WriteMultipleRegisters:
                    expectedBytes = 4;
                    break;
                case FunctionCode.ReadDeviceIdentification:
                    expectedBytes = 6;
                    break;
                default:
                    return (false, default);
            };

            var (responseValid, bodyMessage) = await ReadBytes((uint)(expectedBytes + MessageTranslatorExtension.Crc16Length));
            if (!responseValid)
                return (false, default);

            byte[] fullMessage = new byte[MessageTranslatorExtension.ResponseHeadLength + MessageTranslatorExtension.Crc16Length + expectedBytes];
            fullMessage[0] = deviceId;
            fullMessage[1] = (byte)functionCode;
            fullMessage[2] = expectedBytes;

            Array.Copy(bodyMessage, 0, fullMessage, MessageTranslatorExtension.ResponseHeadLength, bodyMessage.Length);

            bool messageValid = fullMessage.CheckResponseMessage();

            if (!messageValid)
                return (false, null);

            ModbusMessageStruct[] modbusReponse = new ModbusMessageStruct[expectedBytes / 2];

            for (int i = 0; i < modbusReponse.Length; i++)
                modbusReponse[i] = new ModbusMessageStruct(bodyMessage[(i * 2)..(i * 2 + 2)]);

            return (true, modbusReponse);
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
