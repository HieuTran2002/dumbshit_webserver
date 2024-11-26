using System;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace MQTT_Helper
{
    public class MQTT_Sub
    {
        private static MqttFactory factory = new MqttFactory();
        public IMqttClient mqttClient = factory.CreateMqttClient();

        public string broker = "192.168.1.106";
        public int port = 1883;
        public string clientId = "CS";
        public string username = "themachine";
        public string password = "headbanger";

        public async void StartMqttsSubscriber(string[] list_topics)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port)
                .WithCredentials(username, password)
                .WithClientId(clientId)
                .WithCleanSession()
                .Build();

            var connectResult = await mqttClient.ConnectAsync(options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                if (list_topics.Length > 0)
                {
                    foreach (string topic in list_topics)
                    {
                        await mqttClient.SubscribeAsync(topic);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to connect to MQTT broker: {connectResult.ResultCode}");
            }
        }
    }


    public class MQTT_Pub
    {
        private MqttFactory factory = new MqttFactory ();
        private IMqttClient mqttClient;
        private bool isConnected = false;

        // I'm too lazy to make getter-setter for these guy, but make them public still work, so be that as it may
        public string address = "192.168.1.106";
        public int port = 1883;
        public string clientId = "CS2";
        public string username = "themachine";
        public string password = "headbanger";

        public async void connect()
        {
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(address, port)
                .WithCredentials(username, password)
                .WithClientId(clientId)
                .WithCleanSession()
                .Build();

            var connectResult = await mqttClient.ConnectAsync(options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                isConnected = true;

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("greeting")
                    .WithPayload("Hello, MQTT!, from CS2")
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();

                await mqttClient.PublishAsync(message);
            }
        }

        public async void pub(string value, string topic)
        {
            if (!isConnected)
            {
                return;
            }

            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(value)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message);
        }
    }
}
