namespace Vaisala.HMP.NET.Enums;

public enum DeviceIDObject : byte {
    VendorName = 0x00,
    ProductCode = 0x01,
    MajorMinorRevision = 0x02,
    VendorUrl = 0x03,
    ProductName = 0x04,
    ModelName = 0x05,
    UserApplicationName = 0x06,
    SerialNumber = 0x80,
    CalibrationDate = 0x81,
    CalibrationText = 0x82
}