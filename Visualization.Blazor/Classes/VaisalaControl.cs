using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vaisala.HMP.NET.Classes;
using Vaisala.HMP.NET.Enums;
using Vaisala.HMP.NET.Models;
using Vaisala.HMP.NET.Models.Messages;
using Visualization.Blazor.Models;

namespace Visualization.Blazor.Classes
{
    public class VaisalaControl {

        private static CommunicationClass communication;
        private static ConcurrentQueue<ExceptionModel> _exceptions = new();
        private static Thread _recordThread;
        private static Thread _aggregationThread;
        private static bool _cancellationToken;
        private const string _defaultDirectory = "data";
        private ConcurrentQueue<UnaggregatedRecordStruct> _recordsQueue = new();
        
        private const string _tableCreateQuery = @"CREATE TABLE probe_report (id INTEGER PRIMARY KEY, timestamp INTEGER NOT NULL, date_time TEXT NOT NULL, relative_humidity REAL NOT NULL, relative_humidity_sd REAL NOT NULL, relative_humidity_skew REAL NOT NULL, absolute_humidity REAL NOT NULL, absolute_humidity_sd REAL NOT NULL, absolute_humidity_skew REAL NOT NULL, temperature REAL NOT NULL, temperature_sd REAL NOT NULL, temperature_skew REAL NOT NULL, water_concentration REAL NOT NULL, water_concentration_sd REAL NOT NULL, water_concentration_skew REAL NOT NULL);";

        #region Private methods

        private void AggreagationThread() {
            while (!_cancellationToken) {
                Thread.Sleep(AggregateTime);

                List<UnaggregatedRecordStruct> records = new();
                lock (_recordsQueue) {
                    records = _recordsQueue.ToList();
                    _recordsQueue.Clear();
                }

                if (records.Count != 0) {
                    AggregatedRecordModel aggregatedRecord = new(records);

                    _exceptions.Enqueue(new($"Absolute humidity: {aggregatedRecord.AbsoluteHumidity}"));
                    _exceptions.Enqueue(new($"Relative humidity: {aggregatedRecord.RelativeHumidity}"));
                    _exceptions.Enqueue(new($"Temperature: {aggregatedRecord.Temperature}"));
                    _exceptions.Enqueue(new($"Water Concentration: {aggregatedRecord.WaterConcentration}"));

                    int weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    string filename = Path.Combine(_defaultDirectory, $"{BaseFileName}-{weekNumber}-{DateTime.Now.Year}.db");
                    string connectionString = $"Data Source={filename}";

                    if (!File.Exists(filename))
                        ExecuteNonQuery(_tableCreateQuery, connectionString);

                    string sqlQuery = $"INSERT INTO probe_report (timestamp, date_time, relative_humidity, relative_humidity_sd, relative_humidity_skew, absolute_humidity, absolute_humidity_sd, absolute_humidity_skew, temperature, temperature_sd, temperature_skew, water_concentration, water_concentration_sd, water_concentration_skew) " +
                        $"VALUES ({aggregatedRecord.Timestamp},'{aggregatedRecord.Date}',{aggregatedRecord.RelativeHumidity},{aggregatedRecord.RelativeHumiditySd},{aggregatedRecord.RelativeHumiditySkew}," +
                        $"{aggregatedRecord.AbsoluteHumidity},{aggregatedRecord.AbsoluteHumiditySd},{aggregatedRecord.AbsoluteHumiditySkew}," +
                        $"{aggregatedRecord.Temperature},{aggregatedRecord.TemperatureSd},{aggregatedRecord.TemperatureSkew}," +
                        $"{aggregatedRecord.WaterConcentration},{aggregatedRecord.WaterConcentrationSd},{aggregatedRecord.WaterConcentrationSkew})";

                    ExecuteNonQuery(sqlQuery, connectionString);
                }
            }
        }

        public void ExecuteNonQuery(string sqlQuery, string connectionString) {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sqlQuery;

            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine($"SQL Query: {sqlQuery}");
                Console.WriteLine();
            }
            command.Dispose();

            connection.Close();
            connection.Dispose();
        }

        private void RecordThread() {
            while (!_cancellationToken) {

                bool measurementSuccessfull = false;
                
                Task measurementTask = Task.Run(async () => {
                    float absHum = await communication.GetValue<float>(MeasurementFloatMessages.AbsoluteHumidity);
                    float relHum = await communication.GetValue<float>(MeasurementFloatMessages.RelativeHumidity);
                    float temp = await communication.GetValue<float>(MeasurementFloatMessages.Temperature);
                    float waterConc = await communication.GetValue<float>(MeasurementFloatMessages.WaterConcentration);

                    _recordsQueue.Enqueue(new(absHum: absHum, relHum: relHum, temp: temp, waterConc: waterConc));

                    measurementSuccessfull = true;
                });

                Task timeoutTask = Task.Run(() => Thread.Sleep(2000));

                Task.WaitAny(measurementTask, timeoutTask);

                if (!measurementSuccessfull)
                    Console.WriteLine("Faulty measurement");

                Thread.Sleep(2000);
            }
        }

        #endregion

        #region Public methods

        public bool Connected() => communication != null && communication.Connected;

        public void GetSerialPortList() => AvailableSerialPorts = SerialPort.GetPortNames();

        public void Initialize() {
            if (!Directory.Exists(_defaultDirectory))
                Directory.CreateDirectory(_defaultDirectory);

            GetSerialPortList();
        }

        public List<string> PullExceptions() {

            List<string> returnVar = new ();

            if (communication != null)
                while (communication.Exceptions.TryDequeue(out ExceptionModel value))
                    returnVar.Add($"{value.Time}. {value.Message}");

            lock (_exceptions) {
                returnVar.AddRange(_exceptions.Select(value => $"{value.Time}. {value.Message}"));
                _exceptions.Clear();
            }

            return returnVar;
        }

        public void StartRecording() {
            if (_recordThread == null && _aggregationThread == null) {
                Active = true;
                _recordThread = new(new ThreadStart(RecordThread));
                _aggregationThread = new(new ThreadStart(AggreagationThread));
                _cancellationToken = false;
                _recordThread.Start();
                _aggregationThread.Start();
            }
            else
                StopRecording();
        }

        public void StopRecording() {
            _cancellationToken = true;
            _recordThread.Join(); 
            _aggregationThread.Join();
            _recordThread = null;
            _aggregationThread = null;
            Active = false;
        }

        public async Task TryConnect() {
            communication = new CommunicationClass(Port, DeviceId);
            DeviceIdentification = await communication.GetValue(new RequestExtendedStruct(functionCode: FunctionType.ReadDeviceIdentification, mei: MEIType.ReadDeviceInformation, deviceIdCat: DeviceIDCategory.Extended, deviceIdObj: DeviceIDObject.VendorName));
        }

        public void TryDisconnect() {
            communication.Disconnect();
            DeviceIdentification = Array.Empty<DeviceIdentificationModel>();
        }

        #endregion

        #region Public properties

        public bool Active { get; set; } = false;
        public int AggregateTime { get; set; } = 60000;
        public string[] AvailableSerialPorts { get; private set; } = Array.Empty<string>();
        public string BaseFileName { get; set; } = "probe";
        public DeviceIdentificationModel[] DeviceIdentification { get; private set; } = Array.Empty<DeviceIdentificationModel>();
        public byte DeviceId { get; set; } = 240;
        public string Port { get; set; }

        #endregion
    }
}
