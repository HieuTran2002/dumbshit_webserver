using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace MQTT_Helper_iternal
{
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
