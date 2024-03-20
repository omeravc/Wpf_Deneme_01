using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace AdvancedUdpComms
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private Dictionary<string, IPEndPoint> userEndPoints;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUdpClient();
            InitializeUserList();
        }

        private void InitializeUdpClient()
        {
            udpClient = new UdpClient();
            udpClient.BeginReceive(ReceiveCallback, null);
        }

        private void InitializeUserList()
        {
            // Kullanıcı listesi ve IP adresleri burada tanımlanır
            userEndPoints = new Dictionary<string, IPEndPoint>
            {
                { "Kullanıcı1", new IPEndPoint(IPAddress.Parse("192.168.1.2"), 11000) },
                { "Kullanıcı2", new IPEndPoint(IPAddress.Parse("192.168.1.3"), 11000) },
                // Daha fazla kullanıcı eklenebilir
            };

            // Kullanıcı listesini UI'a bağla
            UserListBox.ItemsSource = userEndPoints.Keys;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedBytes = udpClient.EndReceive(ar, ref remoteEndPoint);
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);

            // UI thread üzerinde güncelleme yapmak için Dispatcher kullanın
            Dispatcher.Invoke(() =>
            {
                MessagesTextBox.AppendText($"[{remoteEndPoint}]: {receivedMessage}\n");
            });

            // Veri almaya devam et
            udpClient.BeginReceive(ReceiveCallback, null);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserListBox.SelectedItem != null)
            {
                string selectedUser = UserListBox.SelectedItem.ToString();
                string messageToSend = MessageTextBox.Text;
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);

                // Seçilen kullanıcının IP adresine mesaj gönder
                udpClient.Send(bytesToSend, bytesToSend.Length, userEndPoints[selectedUser]);
            }
        }
    }
}
