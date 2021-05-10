using System;
using Vaisala.HMP.Enums;

namespace Vaisala.HMP.Models
{
    public struct RequestStruct
    {
        public RequestStruct(ushort registerStartAddress, ushort registerCount, FunctionCode code, Type expectedReturnType) {
            Code = code;
            RegisterCount = registerCount;
            RegisterStartAddress = registerStartAddress;
            ExpectedReturnType = expectedReturnType;
        }

        public FunctionCode Code { get; set; }
        public Type ExpectedReturnType { get; set; }
        public ushort RegisterStartAddress { get; set; }
        public ushort RegisterCount { get; set; }

    }
}
