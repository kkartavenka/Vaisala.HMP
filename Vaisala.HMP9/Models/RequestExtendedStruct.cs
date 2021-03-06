using Vaisala.HMP.Enums;

namespace Vaisala.HMP.Models
{
    public struct RequestExtendedStruct
    {
        public RequestExtendedStruct(FunctionType functionCode, MEIType mei, DeviceIDCategory deviceIdCat, DeviceIDObject deviceIdObj) {
            FunctionCode = functionCode;
            MEI = mei;
            DeviceIdCat = deviceIdCat;
            DeviceIdObj = deviceIdObj;
        }

        public DeviceIDCategory DeviceIdCat { get; }
        public DeviceIDObject DeviceIdObj { get; }
        public FunctionType FunctionCode { get; }
        public MEIType MEI { get; }
        
    }
}
