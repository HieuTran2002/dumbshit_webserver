using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Windows.Forms;
using System.Threading;

namespace MQTT_Helper
{
    public class MQTT_Sub
    {
        private static MqttFactory factory = new MqttFactory();
        public IMqttClient mqttClient = factory.CreateMqttClient();

        public string broker = "localhost";
        public int port = 1883;
        public string clientId = "CS";
        bool isConnected = false;
        MqttClientOptions options;

        public async void StartMqttsSubscriber(string[] list_topics)
        {
            var connectResult = await connect();

            MessageBox.Show(connectResult.ResultCode.ToString());

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

            // on lost connection handle
            mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync;
        }

        private async Task<MqttClientConnectResult> connect()
        {
            options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port)
                .WithClientId(clientId)
                .WithCleanSession()
                .WithTimeout(TimeSpan.FromSeconds(5))
                .Build();

            MqttClientConnectResult connectResult = new MqttClientConnectResult();
            while (!isConnected)
            {
                try
                {
                    connectResult = await mqttClient.ConnectAsync(options);
                    isConnected = true;
                }
                catch (Exception ex) 
                {
                }
            }
            return connectResult;
        }

        private async Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {            
            isConnected = false;
            await Task.Delay(TimeSpan.FromSeconds(5));
            await mqttClient.ConnectAsync(options);
            isConnected = true;
        }
    }

    public class MQTT_Pub
    {
        private MqttFactory factory = new MqttFactory ();
        private IMqttClient mqttClient;
        private bool isConnected = false;

        // I'm too lazy to make getter-setter for these guy, but make them public still work, so be that as it may
        public string address = "localhost";
        public int port = 1883;
        public string clientId = "CS2";

        public async void connect()
        {
            mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(address, port)
                .WithClientId(clientId)
                .WithCleanSession()
                .WithTimeout(TimeSpan.FromSeconds(5))
                .Build();

            while(!isConnected)
            {
                try
                {
                    var connectResult = await mqttClient.ConnectAsync(options);
                    isConnected = true;
                }
                catch (Exception ex) 
                {
                }


                if (isConnected)
                {
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic("greeting")
                        .WithPayload("Hello, MQTT!, from CS2")
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                        .WithRetainFlag()
                        .Build();

                    await mqttClient.PublishAsync(message);
                    mqttClient.DisconnectedAsync += MqttClient_DisconnectedAsync; ;
                }
            }
        }

        private async Task MqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(address, port)
                .WithClientId(clientId)
                .WithCleanSession()
                .WithTimeout(TimeSpan.FromSeconds(5))
                .Build();

            isConnected = false;
            await Task.Delay(TimeSpan.FromSeconds(5));
            await mqttClient.ConnectAsync(options);
            isConnected = true;
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
