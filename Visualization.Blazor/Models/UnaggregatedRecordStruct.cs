namespace Visualization.Blazor.Models
{
    public struct UnaggregatedRecordStruct
    {
        public UnaggregatedRecordStruct(float absHum, float relHum, float temp, float waterConc) {
            AbsoluteHumidity = absHum;
            RelativeHumidity = relHum;
            Temperature = temp;
            WaterConcentration = waterConc;
        }

        public float AbsoluteHumidity { get; }
        public float RelativeHumidity { get; }
        public float Temperature { get; }
        public float WaterConcentration { get; }
    }
}
