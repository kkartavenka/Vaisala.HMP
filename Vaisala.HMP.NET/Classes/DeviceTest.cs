using System;
using Vaisala.HMP.NET.Models;

namespace Vaisala.HMP.NET.Classes;

public class DeviceTest {
    private const int _test1 = -12345;
    private const double _test2 = -123.45;
    private const string _test3 = "-123.45";

    public DeviceTest(ModbusMessageStruct[] test1, ModbusMessageStruct[] test2, ModbusMessageStruct[] test3) {
        Test1 = _test1 == test1.GetNumeric<short>();

        if (Math.Abs(_test2 - test2.GetNumeric<float>(true)) < 1e-3)
            ReverseOrderFloat = true;
        else if (Math.Abs(_test2 - test2.GetNumeric<float>(false)) < 1e-3)
            ReverseOrderFloat = false;
        else
            Test2 = false;

        Test3 = _test3 == test3.GetString();

        IsSuccess = Test1 & Test2 & Test3;
    }

    public bool ReverseOrderFloat { get; private set; }

    public bool Test1 { get; private set; }
    public bool Test2 { get; private set; } = true;
    public bool Test3 { get; private set; }

    public bool IsSuccess { get; private set; }
}

