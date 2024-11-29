using System;
using System.Threading;
using System.Windows.Forms;
using RAM;
using MySqlConnector;
using MQTTnet;
using System.Threading.Tasks;
using MQTT_Helper;

namespace whatever
{
    public partial class Form1 : Form
    {
        static string connectionString = "Server=192.168.1.106;Database=thedb;Uid=theuser;Pwd=hieu;SslMode=None";
        private MySqlConnection connection = new MySqlConnection(connectionString);
         
        RAM_Handle rAM = new RAM_Handle();
        static private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;
        MQTT_Sub sub = new MQTT_Sub();

        mysql sql = new mysql();

        string[] sub_topics = {"furnace_1", "furnace_2", "furnace_3", "furnace_4"};

        public Form1()
        {
            InitializeComponent();

            sub.StartMqttsSubscriber(sub_topics);
            StartDataProcessing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void StartDataProcessing()
        {
            ThreadPool.QueueUserWorkItem(state => DataHandlingProcess());
        }

        private void DataHandlingProcess()
        {
            int i = 0;
            while (!token.IsCancellationRequested)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // this.label1.Text = rAM.getRam().ToString();
                    this.label1.Text = i.ToString();
                });
                i++;
                Thread.Sleep(1000);
            }
        }
        private void StopDataProcessing()
        {
            cancellationTokenSource.Cancel();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDataProcessing();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state => {
                sql.openConn();
            });

            sub.mqttClient.ApplicationMessageReceivedAsync += a =>
            {
                // MessageBox.Show(a.ApplicationMessage.Topic + "\n" + a.ApplicationMessage.ConvertPayloadToString());
                sql.update(a.ApplicationMessage.Topic, a.ApplicationMessage.ConvertPayloadToString());

                return Task.CompletedTask;
            };
        }
    }
}
