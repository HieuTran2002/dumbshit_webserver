using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace whatever
{
    internal class mysql

    {
        static string connectionString = "Server=192.168.1.106;Database=thedb;Uid=theuser;Pwd=hieu;SslMode=None";
        private MySqlConnection connection = new MySqlConnection(connectionString);
        private bool isConnected = false;

        public void openConn()
        {
            try
            {
                connection.Open();
                isConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void update(string topic, string value)
        {
            if (!isConnected)
            {
                return;
            }

            string query = "INSERT INTO device_data (device_name, data_value, timestamp_column) VALUES (@deviceName, @dataValue, NOW())";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@deviceName", topic);
                command.Parameters.AddWithValue("@dataValue", value);
                command.ExecuteNonQuery();
            }
        }
    }
}
