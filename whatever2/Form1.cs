using System;
using System.Threading;
using System.Windows.Forms;
using MQTT_Helper;

namespace whatever2
{
    public partial class Form1 : Form
    {
        static private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        RAMGetter RAMGetter = new RAMGetter();
        MQTT_Pub pub = new MQTT_Pub();
        public Form1()
        {
            InitializeComponent();
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
                    float[] RAMArray = RAMGetter.GetRAM();

                    // Format to 2 decimal places
                    string total_str = RAMArray[1].ToString("F2");
                    string available_str = RAMArray[2].ToString("F2");
                    string used_percentage_str = RAMArray[3].ToString("F2");

                    this.textBox1.Text = RAMArray[0].ToString("F2");
                    this.textBox2.Text = RAMArray[1].ToString("F2");
                    this.textBox3.Text = RAMArray[2].ToString("F2");
                    this.textBox4.Text = RAMArray[3].ToString("F2");

                    for (global::System.Int32 j = 0; j < 4; j++)
                    {
                        pub.pub(RAMArray[j].ToString("F2"), $"furnace_{j + 1}");
                    }
                });
                i++;
                Thread.Sleep(1000);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartDataProcessing();
            pub.connect();
        }
    }
}
