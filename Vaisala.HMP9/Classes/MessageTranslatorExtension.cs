using System;
using System.Text;
using Vaisala.HMP.Models;

namespace Vaisala.HMP.Classes
{
    public static class MessageTranslatorExtension
    {
        private static readonly char[] _trimChars = new char[] { ' ', '\t', '\0', '\r', '\n' };
        public static int ResponseHeadLength = 3;
        public static int RequestHeadLength = 2;
        public static int Crc16Length = 2;

        public static bool CheckResponseMessage(this byte[] message) {
            byte[] crc = message.GetCrc16();
            int offset = message.Length - Crc16Length;
            for (int i = offset; i < message.Length; i++)
                if (message[i] != crc[i - offset])
                    return false;

            return true;
        }

        public static byte[] GetBytes(this ushort value, bool isLittleEndian) => isLittleEndian != BitConverter.IsLittleEndian ? BitConverter.GetBytes(value).ReverseArray() : BitConverter.GetBytes(value);

        public static byte[] GetCrc16(this byte[] value) {
            ushort crc16 = 0xFFFF;
            byte lsb;

            for (int i = 0; i < value.Length - Crc16Length; i++) {
                crc16 = (ushort)(crc16 ^ value[i]);
                for (int j = 0; j < 8; j++) {
                    lsb = (byte)(crc16 & 1);
                    crc16 = (ushort)(crc16 >> 1);
                    if (lsb == 1)
                        crc16 = (ushort)(crc16 ^ 0xA001);
                }
            }

            return new byte[] { (byte)crc16, (byte)(crc16 >> 8) };
        }

        public static T GetNumeric<T>(this ModbusMessageStruct[] modbusMessage, bool reverseOrder = true) {

            byte[] byteMessage = new byte[modbusMessage.Length * 2];

            for (int i = 0; i < modbusMessage.Length; i++) {
                byteMessage[i * 2] = modbusMessage[reverseOrder ? modbusMessage.Length - i - 1 : i].HighByte;
                byteMessage[i * 2 + 1] = modbusMessage[reverseOrder ? modbusMessage.Length - i - 1 : i].LowByte;
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(byteMessage);

            switch (typeof(T)) {
                case Type int32Type when int32Type == typeof(int):
                    if (modbusMessage.Length != 2)
                        return default;

                    return (T)Convert.ChangeType(BitConverter.ToInt32(byteMessage), typeof(T));

                case Type int16Type when int16Type == typeof(short):
                    if (modbusMessage.Length != 1)
                        return default;

                    return (T)Convert.ChangeType(BitConverter.ToInt16(byteMessage), typeof(T));

                case Type floatType when floatType == typeof(float):
                    if (modbusMessage.Length != 2)
                        return default;

                    return (T)Convert.ChangeType(BitConverter.ToSingle(byteMessage), typeof(T));

                case Type doubleType when doubleType == typeof(float):
                    if (modbusMessage.Length != 4)
                        return default;

                    return (T)Convert.ChangeType(BitConverter.ToDouble(byteMessage), typeof(T));

                default:
                    return default;
            }
        }

        public static string GetString(this ModbusMessageStruct[] modbusMessage, bool flipBytes = false) {

            byte[] byteMessage = new byte[modbusMessage.Length * 2];

            for (int i = 0; i < modbusMessage.Length; i++) {
                byteMessage[i * 2] = flipBytes ? modbusMessage[i].LowByte : modbusMessage[i].HighByte;
                byteMessage[i * 2 + 1] = flipBytes ? modbusMessage[i].HighByte : modbusMessage[i].LowByte;
            }

            string stringMessage = Encoding.ASCII.GetString(byteMessage).Trim(_trimChars);

            return stringMessage;
        }

        public static byte[] ReverseArray(this byte[] array) {
            Array.Reverse(array);
            return array;
        }

        public static byte[] ToBytes(this RequestStruct message, byte deviceId, bool isLittleEndian) {

            byte[] address = message.RegisterStartAddress.GetBytes(isLittleEndian);
            byte[] count = message.RegisterCount.GetBytes(isLittleEndian);

            byte[] baseArray = new byte[RequestHeadLength + address.Length + count.Length + Crc16Length];
            baseArray[0] = deviceId;
            baseArray[1] = (byte)message.Code;

            Array.Copy(address, 0, baseArray, RequestHeadLength, address.Length);
            Array.Copy(count, 0, baseArray, RequestHeadLength + address.Length, count.Length);

            byte[] crc16 = baseArray.GetCrc16();

            Array.Copy(crc16, 0, baseArray, baseArray.Length - Crc16Length, Crc16Length);

            return baseArray;
        }

    }

}
