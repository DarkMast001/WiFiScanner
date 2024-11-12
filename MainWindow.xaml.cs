using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.ComponentModel;
using WiFiScanner.Models;
using NativeWifi;
using System.Data.SQLite;

namespace WiFiScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<Network> _networkList;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void ButtonScan_Click(object sender, RoutedEventArgs e)
        {
            string bestNet = null;
            uint maxQuality = 0;

            if (_networkList != null) 
            {
                _networkList.Clear();
            }
            _networkList = new BindingList<Network>();

            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                {
                    string ssid = System.Text.Encoding.ASCII.GetString(network.dot11Ssid.SSID).TrimEnd('\0');
                    _networkList.Add(new Network() { Name = ssid, SignalLevel = network.wlanSignalQuality.ToString() });
                    if (network.wlanSignalQuality > maxQuality)
                    {
                        maxQuality = network.wlanSignalQuality;
                        bestNet = ssid;
                    }
                }
            }

            WiFiNetsList.ItemsSource = _networkList;

            if (bestNet != null)
            {
                labelBestNet.Content = bestNet;
            }
            else
            {
                labelBestNet.Content = "Сети не обнаружено";
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Если будите у себя запускать, то изменить путь до WiFiDataBase.db
            string databasePath = @"D:\Internship_XTECH\WiFiScanner\DataBase\WiFiDataBase.db";

            string connectionString = $"Data Source={databasePath};Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM WiFiNets";
                using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection))
                {
                    int rowsDeleted = deleteCommand.ExecuteNonQuery();
                }

                foreach (Network network in _networkList)
                {
                    string networkName = network.Name ?? "null";
                    int signalLevel = int.Parse(network.SignalLevel ?? "0");
                    string insertQuery = "INSERT INTO WiFiNets (name, signalLevel) VALUES (@name, @signalLevel)";
                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        // Задание значений для параметров запроса
                        command.Parameters.AddWithValue("@name", networkName);
                        command.Parameters.AddWithValue("@signalLevel", signalLevel);

                        // Выполнение команды
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }
    }
}