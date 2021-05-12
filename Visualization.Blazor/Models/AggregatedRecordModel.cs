using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Visualization.Blazor.Models
{
    public class AggregatedRecordModel
    {
        public AggregatedRecordModel(List<UnaggregatedRecordStruct> unaggregatedData) {
            AbsoluteHumidity = unaggregatedData.Select(m => m.AbsoluteHumidity).Mean();
            AbsoluteHumiditySd = unaggregatedData.Select(m => m.AbsoluteHumidity).StandardDeviation();
            AbsoluteHumiditySkew = unaggregatedData.Select(m => (double)m.AbsoluteHumidity).Skewness();

            RelativeHumidity = unaggregatedData.Select(m => m.RelativeHumidity).Mean();
            RelativeHumiditySd = unaggregatedData.Select(m => m.RelativeHumidity).StandardDeviation();
            RelativeHumiditySkew = unaggregatedData.Select(m => (double)m.RelativeHumidity).Skewness();

            WaterConcentration = unaggregatedData.Select(m => m.WaterConcentration).Mean();
            WaterConcentrationSd = unaggregatedData.Select(m => m.WaterConcentration).StandardDeviation();
            WaterConcentrationSkew = unaggregatedData.Select(m => (double)m.WaterConcentration).Skewness();

            Temperature = unaggregatedData.Select(m => m.Temperature).Mean();
            TemperatureSd = unaggregatedData.Select(m => m.Temperature).StandardDeviation();
            TemperatureSkew = unaggregatedData.Select(m => (double)m.Temperature).Skewness();

            Date = DateTime.Now;
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public double AbsoluteHumidity { get; }
        public double AbsoluteHumiditySd { get; }
        public double AbsoluteHumiditySkew { get; }

        public double RelativeHumidity { get; }
        public double RelativeHumiditySd { get; }
        public double RelativeHumiditySkew { get; }

        public double Temperature { get; }
        public double TemperatureSd { get; }
        public double TemperatureSkew { get; }

        public DateTime Date { get; }
        public long Timestamp { get; }

        public double WaterConcentration { get; }
        public double WaterConcentrationSd { get; }
        public double WaterConcentrationSkew { get; }
    }
}
