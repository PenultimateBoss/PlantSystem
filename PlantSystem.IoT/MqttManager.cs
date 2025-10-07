#nullable enable

using System;
using System.Threading;
using System.Collections;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System.Security.Cryptography.X509Certificates;

namespace PlantSystem.IoT
{
    public sealed class MqttManager : IDisposable
    {
        #region Static
        private static MqttManager? instance;

        public bool Initialized
        {
            get => instance is not null;
        }
        public static MqttManager Instance
        {
            get => instance ??= new MqttManager();
        }
        #endregion

        #region Instance
        private X509Certificate HiveCert { get; }
        private X509Certificate ClientCert { get; }
        private MqttClient MQTT { get; }
        private ArrayList MessageList { get; }
        private string[] SubTopics { get; }
        private MqttQoSLevel[] TopicQoSs { get; }
        private ArrayList UserProps { get; }

        private MqttManager()
        {
            HiveCert = new X509Certificate(Resources.GetString(Resources.StringResources.isrgrootx1));
            ClientCert = new X509Certificate();
            MQTT = new MqttClient("62df834cd0c74c3bbfdf9d5e3ec98238.s1.eu.hivemq.cloud", 8883, true, HiveCert, null, MqttSslProtocols.TLSv1_2);
            MessageList = new ArrayList();
            SubTopics = new string[]
            { 
                "plant/response"
            };
            TopicQoSs = new MqttQoSLevel[]
            {
                MqttQoSLevel.AtLeastOnce
            };
            UserProps = new ArrayList();
        }

        private byte[] MakeMessage()
        {
            int length = 0;
            foreach(byte[] msg in MessageList)
            {
                length += msg.Length;
            }
            byte[] message = new byte[length];
            int offset = 0;
            foreach(byte[] msg in MessageList)
            {
                Array.Copy(msg, 0, message, offset, msg.Length);
                offset += msg.Length;
            }
            return message;
        }
        public void SendReceive(byte[] message, out byte[]? response)
        {
            byte[]? res = null;
            try
            {
                MQTT.MqttMsgPublishReceived += ReceiveMessage;
                MQTT.Connect("IoT", "NF-Client", "NFx00000", true, 60);
                MQTT.Subscribe(SubTopics, TopicQoSs);
                MessageList.Add(message);
                message = MakeMessage();
                MQTT.Publish("plant/sensors", message, "", UserProps, MqttQoSLevel.AtLeastOnce, false);
                Thread.Sleep(10000);
                MessageList.Clear();
                MQTT.Disconnect();
                response = res;
            }
            catch
            {
                MessageList.Add(message);
                response = null;
                return;
            }
            finally
            {
                MQTT.MqttMsgPublishReceived -= ReceiveMessage;
            }         

            void ReceiveMessage(object sender, MqttMsgPublishEventArgs event_args)
            {
                if(event_args.Topic is "plant/response")
                {
                    res = event_args.Message;
                }
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            MQTT.Dispose();
            instance = null;
        }
        #endregion
    }
}