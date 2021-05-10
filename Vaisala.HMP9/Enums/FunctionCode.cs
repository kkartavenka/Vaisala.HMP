namespace Vaisala.HMP.Enums
{
    public enum FunctionCode : byte
    {
        ReadHoldingRegisters = 0x03,
        WriteMultipleRegisters = 0x10,
        ReadDeviceIdentification = 0x2B,
        Error = 0x80
    }
}
