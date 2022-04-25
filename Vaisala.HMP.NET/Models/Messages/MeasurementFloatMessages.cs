using Vaisala.HMP.NET.Enums;

namespace Vaisala.HMP.NET.Models.Messages;

public static class MeasurementFloatMessages {
    public static RequestStruct RelativeHumidity => new RequestStruct(registerStartAddress: 0x0000, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct Temperature => new RequestStruct(registerStartAddress: 0x0002, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct DewPointTemperature => new RequestStruct(registerStartAddress: 0x0006, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct DewFrostPointTemperature => new RequestStruct(registerStartAddress: 0x0008, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct DewFrostPointTemperatureAt1Atm => new RequestStruct(registerStartAddress: 0x000A, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct DewPointTemperatureAt1Atm => new RequestStruct(registerStartAddress: 0x000C, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct AbsoluteHumidity => new RequestStruct(registerStartAddress: 0x000E, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct MixingRatio => new RequestStruct(registerStartAddress: 0x0010, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WetBulbTemperature => new RequestStruct(registerStartAddress: 0x0012, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));


    public static RequestStruct WaterConcentration => new RequestStruct(registerStartAddress: 0x0014, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterVaporPressure => new RequestStruct(registerStartAddress: 0x0016, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterVaporSaturationPressure => new RequestStruct(registerStartAddress: 0x0018, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct Enthalpy => new RequestStruct(registerStartAddress: 0x001A, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterActivity => new RequestStruct(registerStartAddress: 0x001C, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct DewPointTemperatureDifference => new RequestStruct(registerStartAddress: 0x001E, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct AbsoluteHumidityAtNTP => new RequestStruct(registerStartAddress: 0x0020, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterConcentrationInOil => new RequestStruct(registerStartAddress: 0x0022, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct RelativeSaturation => new RequestStruct(registerStartAddress: 0x0028, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterConcentrationWetBasis => new RequestStruct(registerStartAddress: 0x002A, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct RelativeHumidityDewDivFrost => new RequestStruct(registerStartAddress: 0x002C, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));
    public static RequestStruct WaterMassFraction => new RequestStruct(registerStartAddress: 0x0040, registerCount: 2, code: FunctionType.ReadHoldingRegisters, expectedReturnType: typeof(float));

}

