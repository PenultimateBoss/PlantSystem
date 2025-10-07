#nullable enable

using System;
using System.Threading;
using nanoFramework.Networking;

namespace PlantSystem.IoT
{
    public static class Program
    {
        public static void Main()
        {
            byte measuring_period = 1;
            WifiNetworkHelper.ConnectDhcp("CISPE-PHONE", "11111111");
            while(true)
            {
                PlantSensors.Instance.GetAllData(out byte light, out byte moisture, out byte temperature, out byte humidity);
                byte[] message = new byte[]
                {
                    light,
                    moisture,
                    temperature,
                    humidity,
                    measuring_period,
                };
                MqttManager.Instance.SendReceive(message, out byte[]? response);
                if(response is not null && response.Length >= 1)
                {
                    measuring_period = response[0];
                    if(measuring_period <= 0)
                    {
                        measuring_period = 1;
                    }
                }
                Thread.Sleep(TimeSpan.FromMinutes(measuring_period));
            }
        }
    }
}