#nullable enable

using System;
using System.Device.Adc;
using Iot.Device.DHTxx.Esp32;
using nanoFramework.Networking;
using nanoFramework.Hardware.Esp32;

namespace PlantSystem.IoT
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start");
            AdcController adc = new();
            AdcChannel photoresistor = adc.OpenChannel(4);
            AdcChannel moisture = adc.OpenChannel(5);
            Dht11 dht11 = new(25, 26);
            while(true)
            {
                Console.WriteLine("Gathering data...");
                Console.WriteLine($"Light: {photoresistor.ReadRatio() * 100}%");
                Console.WriteLine($"Moisture: {moisture.ReadRatio() * 100}%");
                Console.WriteLine($"Temperature: {dht11.Temperature.DegreesCelsius}C");
                Console.WriteLine($"Humidity: {dht11.Humidity.Percent}%");

                Console.WriteLine("LightSleep");
                WifiNetworkHelper.Disconnect();
                Sleep.EnableWakeupByTimer(TimeSpan.FromSeconds(10));
                Sleep.StartLightSleep();
            }
        }
    }
}
