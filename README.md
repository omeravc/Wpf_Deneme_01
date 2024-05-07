using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321; // Sabit dinleme portu
        private const int targetPort = 12345; // Sabit hedef port

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            MessageBox.Show("UDP Client is listening on port " + listenPort);
        }

        private void ReceiveCallback(IAsyncResult i)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(i, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            Dispatcher.Invoke(() => messagesTextBox.Text += $"Received: {message}\n");
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{timestamp} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                messagesTextBox.Text += $"Sent: {messageToSend}\n";
                messageTextBox.Clear(); // Clears the message box after sending
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}


---------------------------------------------

<Window x:Class="UdpChatApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="UDP Chat App" Height="400" Width="600">
    <StackPanel Background="#F0F0F0">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="Enter Target IP" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="DarkGreen" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="LightGreen"/>
        <ScrollViewer Height="200" Margin="10">
            <TextBox x:Name="messagesTextBox" IsReadOnly="True" Background="WhiteSmoke" TextWrapping="Wrap" Foreground="DarkRed"/>
        </ScrollViewer>
    </StackPanel>
</Window>
