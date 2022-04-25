using Vaisala.HMP.NET.Enums;

namespace Vaisala.HMP.NET.Models;

public class DeviceIdentificationModel {
    public DeviceIdentificationModel(int objectId, string value) {
        ObjectName = ((DeviceIDObject)objectId).ToString();
        Value = value;
    }
    public string ObjectName { get; set; }
    public string Value { get; set; }
}
