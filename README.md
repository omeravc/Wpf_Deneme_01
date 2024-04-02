using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp2024._04._02
{
    public partial class AnaPencere : Window
    {
        private const int Port = 11000;
        private UdpClient _udpClient;
        private Thread _listeningThread;
        private Dictionary<string, IPEndPoint> _onlineUsers;
        private Dictionary<string, string> _messageHistory;

        public AnaPencere()
        {
            InitializeComponent();
            _udpClient = new UdpClient(Port);
            _onlineUsers = new Dictionary<string, IPEndPoint>();
            _messageHistory = new Dictionary<string, string>();
            _listeningThread = new Thread(ListenForMessages);
            _listeningThread.Start();
            LoadUsers(); // Kullanıcıları yükle
        }

        private void ListenForMessages()
        {
            try
            {
                while (true)
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    byte[] receivedBytes = _udpClient.Receive(ref remoteIpEndPoint);
                    string message = Encoding.ASCII.GetString(receivedBytes);

                    Dispatcher.Invoke(() => HandleReceivedMessage(message, remoteIpEndPoint));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void HandleReceivedMessage(string message, IPEndPoint senderEndPoint)
        {
            string senderUsername = _onlineUsers.FirstOrDefault(x => x.Value.Equals(senderEndPoint)).Key;
            string formattedMessage = $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - {senderUsername}: {message}\n";
            textBoxMessages.Text += formattedMessage; // Eski mesajların üzerine eklenir
            AddMessageToHistory(senderUsername, formattedMessage);
        }

        private void AddMessageToHistory(string username, string message)
        {
            if (!_messageHistory.ContainsKey(username))
                _messageHistory.Add(username, "");

            _messageHistory[username] += message;
        }

        private void SendMessage(string message, IPEndPoint receiverEndPoint)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            _udpClient.Send(bytesToSend, bytesToSend.Length, receiverEndPoint);
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessageToSelectedUser();
        }

        private void TextBoxMessage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessageToSelectedUser();
            }
        }

        private void SendMessageToSelectedUser()
        {
            if (listBoxUsers.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.");
                return; // Bir kullanıcı seçilmediyse gönderme işlemi iptal edilir.
            }

            string selectedUsername = listBoxUsers.SelectedItem.ToString();
            IPEndPoint receiverEndPoint = _onlineUsers[selectedUsername];
            string messageToSend = textBoxMessage.Text;

            SendMessage(messageToSend, receiverEndPoint);

            textBoxMessages.Text += $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - Ben: {messageToSend}\n";
            AddMessageToHistory(selectedUsername, $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} - Ben: {messageToSend}\n");
            textBoxMessage.Clear();
        }

        private void LoadUsers()
        {
            _onlineUsers.Clear();
            listBoxUsers.Items.Clear();

            _onlineUsers.Add("Kullanıcı1", new IPEndPoint(IPAddress.Parse("192.168.1.101"), Port));
            _onlineUsers.Add("Kullanıcı2", new IPEndPoint(IPAddress.Parse("192.168.1.102"), Port));

            foreach (var username in _onlineUsers.Keys)
            {
                listBoxUsers.Items.Add(username);
            }
        }

        private void ListBoxUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxUsers.SelectedItem != null)
            {
                string selectedUsername = listBoxUsers.SelectedItem.ToString();
                textBoxMessages.Text = _messageHistory.ContainsKey(selectedUsername) ? _messageHistory[selectedUsername] : "";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _udpClient.Close();
            _listeningThread.Abort();
        }
    }
}



//////////////////////////////////////////////////


<Window x:Class="WpfApp2024._04._02.AnaPencere"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="UDP Sohbet Uygulaması" Height="450" Width="600">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <!-- Kullanıcılar ListBox -->
        <ListBox x:Name="listBoxUsers" Grid.Column="0" Margin="10" SelectionChanged="ListBoxUsers_SelectionChanged"/>

        <!-- Mesajlar için TextBox -->
        <TextBox x:Name="textBoxMessages" Grid.Column="1" Margin="10" VerticalScrollBarVisibility="Auto" HorizontalScrollBarVisibility="Auto" IsReadOnly="True"/>

        <!-- Mesaj girişi ve Gönder düğmesi -->
        <StackPanel Grid.Column="1" Margin="10" VerticalAlignment="Bottom">
            <TextBox x:Name="textBoxMessage" Height="100" TextWrapping="Wrap" VerticalScrollBarVisibility="Auto" AcceptsReturn="True" KeyDown="TextBoxMessage_KeyDown"/>
            <Button Content="Gönder" Click="ButtonSend_Click" HorizontalAlignment="Right" Margin="0 5 0 0"/>
        </StackPanel>
    </Grid>
</Window>
