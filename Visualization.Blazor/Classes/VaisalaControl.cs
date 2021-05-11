using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Vaisala.HMP.Classes;
using Vaisala.HMP.Enums;
using Vaisala.HMP.Models;

namespace Visualization.Blazor.Classes
{
    public class VaisalaControl {
        public static CommunicationClass communication;


        public void GetSerialPortList() => AvailableSerialPorts = SerialPort.GetPortNames();

        public void Initialize() {
            GetSerialPortList();
        }

        public async Task TryConnect() {
            communication = new CommunicationClass(Port, DeviceId);
            DeviceIdentification = await communication.GetValue(new RequestExtendedStruct(functionCode: FunctionType.ReadDeviceIdentification, mei: MEIType.ReadDeviceInformation, deviceIdCat: DeviceIDCategory.Extended, deviceIdObj: DeviceIDObject.VendorName));
        }


        public string[] AvailableSerialPorts { get; private set; } = new string[0];
        public DeviceIdentificationModel[] DeviceIdentification { get; private set; } = new DeviceIdentificationModel[0];
        public byte DeviceId { get; set; } = 240;
        public string Port { get; set; }
    }
}
