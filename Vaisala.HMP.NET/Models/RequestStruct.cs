using System;
using Vaisala.HMP.NET.Enums;

namespace Vaisala.HMP.NET.Models;

public struct RequestStruct {
    public RequestStruct(ushort registerStartAddress, ushort registerCount, FunctionType code, Type expectedReturnType) {
        Code = code;
        RegisterCount = registerCount;
        RegisterStartAddress = registerStartAddress;
        ExpectedReturnType = expectedReturnType;
    }

    public FunctionType Code { get; set; }
    public Type ExpectedReturnType { get; set; }
    public ushort RegisterStartAddress { get; set; }
    public ushort RegisterCount { get; set; }

}
