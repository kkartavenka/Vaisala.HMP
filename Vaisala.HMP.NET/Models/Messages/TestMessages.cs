using Vaisala.HMP.NET.Enums;

namespace Vaisala.HMP.NET.Models.Messages;

public static class TestMessages {
    /// <summary>
    /// Expected value - 12345
    /// </summary>
    public static RequestStruct SignedIntegerTest => new RequestStruct(registerStartAddress: 0x1F00, registerCount: 1, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(short));

    /// <summary>
    /// Expected value -123.45
    /// </summary>
    public static RequestStruct FloatingPointTest => new RequestStruct(registerStartAddress: 0x1F01, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));

    /// <summary>
    /// Expected value "-123.45"
    /// </summary>
    public static RequestStruct TextStringTest => new RequestStruct(registerStartAddress: 0x1F03, registerCount: 4, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(string));
}

