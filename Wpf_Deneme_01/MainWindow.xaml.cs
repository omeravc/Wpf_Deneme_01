using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wpf_Deneme_01

{
    

    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int localPort = 12345;
        private const int remotePort = 54321;
        private const string remoteIpAddress = "127.0.0.1";

        public MainWindow()
        {
            InitializeComponent();
            udpClient = new UdpClient(localPort);
            Task.Run(ReceiveMessages);
        }

        private async Task ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var receiveResult = await udpClient.ReceiveAsync();
                    var receivedMessage = Encoding.UTF8.GetString(receiveResult.Buffer);
                    Dispatcher.Invoke(() =>
                    {
                        incomingMessagesListBox.Items.Add($"Received: {receivedMessage}");
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SendMessage(string message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                udpClient.Send(bytes, bytes.Length, remoteIpAddress, remotePort);
                outgoingMessagesListBox.Items.Add($"Sent: {message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageTextBox.Text))
            {
                SendMessage(messageTextBox.Text);
                messageTextBox.Text = string.Empty;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            udpClient.Close();
            base.OnClosed(e);
        }
    }
}
