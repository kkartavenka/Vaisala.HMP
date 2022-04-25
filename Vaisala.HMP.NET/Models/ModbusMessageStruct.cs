using System;

namespace Vaisala.HMP.NET.Models;

public struct ModbusMessageStruct {
    public ModbusMessageStruct(byte[] values) {
        HighByte = values[0];
        LowByte = values[1];
        Timestamp = DateTime.Now;
    }
    public DateTime Timestamp { get; }
    public byte HighByte { get; }
    public byte LowByte { get; }
}

