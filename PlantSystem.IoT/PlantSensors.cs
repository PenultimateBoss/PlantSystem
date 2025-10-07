#nullable enable

using System;
using System.Device.Adc;
using Iot.Device.DHTxx.Esp32;

namespace PlantSystem.IoT
{
    public sealed class PlantSensors : IDisposable
    {
        #region Static
        private static PlantSensors? instance;

        public bool Initialized
        {
            get => instance is not null;
        }
        public static PlantSensors Instance
        {
            get => instance ??= new PlantSensors();
        }
        #endregion

        #region Instance
        private AdcChannel Photoresistor { get; }
        private AdcChannel MoistureSensor { get; }
        private Dht11 DHT11 { get; set; }

        private PlantSensors()
        {
            AdcController adc_controller = new();
            Photoresistor = adc_controller.OpenChannel(4);
            MoistureSensor = adc_controller.OpenChannel(5);
            DHT11 = new Dht11(25, 26);
        }

        public void GetAllData(out byte light, out byte moisture, out byte temperature, out byte humidity)
        {
            while(true)
            {
                try
                {
                    light = (byte)(Photoresistor.ReadRatio() * 100);
                    moisture = (byte)(MoistureSensor.ReadRatio() * 100);
                    temperature = (byte)DHT11.Temperature.DegreesCelsius;
                    humidity = (byte)DHT11.Humidity.Percent;
                    return;
                }
                catch { }
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            MoistureSensor.Dispose();
            Photoresistor.Dispose();
            DHT11.Dispose();
            instance = null;
        }
        #endregion
    }
}