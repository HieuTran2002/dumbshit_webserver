using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MySqlConnector;

namespace MQTT_Helper_internal
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
}
