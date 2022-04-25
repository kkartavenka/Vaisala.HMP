namespace Vaisala.HMP.NET.Models;

public class ResponseModel {
    public ResponseModel() { }

    public ResponseModel(string exceptionMessage) => ExceptionMessage = exceptionMessage;

    public ResponseModel(ModbusMessageStruct[] holdingRegisters) {
        Success = holdingRegisters != default;
        HoldingRegisters = holdingRegisters;
    }

    public ResponseModel(DeviceIdentificationModel[] deviceIdentification) {
        Success = deviceIdentification != default;
        DeviceIdentification = deviceIdentification;
    }

    public DeviceIdentificationModel[]? DeviceIdentification { get; private set; }
    public ModbusMessageStruct[]? HoldingRegisters { get; private set; }
    public string ExceptionMessage { get; private set; } = string.Empty;
    public bool Success { get; private set; }
}
