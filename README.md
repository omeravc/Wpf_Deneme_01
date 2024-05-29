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

&&&&&&&&&&&&

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            UpdateStatusBar($"UDP Client is listening on port {listenPort}");
        }

        private void UpdateStatusBar(string text)
        {
            statusBarItem.Content = text;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(ar, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            DisplayMessage($"Received: {message}", false);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void DisplayMessage(string message, bool isSentByMe)
        {
            Dispatcher.Invoke(() =>
            {
                var messageItem = new ListBoxItem();
                messageItem.Content = message;
                messageItem.HorizontalContentAlignment = isSentByMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                messageItem.Background = isSentByMe ? Brushes.LightGreen : Brushes.WhiteSmoke;
                messagesListBox.Items.Add(messageItem);
                messageItem.Focus(); // Scroll to the latest message
            });
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

                DisplayMessage($"Sent: {messageToSend}", true);
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
------------
<Window x:Class="UdpChatApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="UDP Chat App" Height="400" Width="600">
    <StackPanel Background="#F0F0F0">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="Enter Target IP" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="DarkGreen" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="LightGreen"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar Height="22" Background="Gray" Foreground="White">
            <StatusBarItem x:Name="statusBarItem" HorizontalAlignment="Left"/>
        </StatusBar>
    </StackPanel>
</Window>

*************************************
<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:Emit="clr-namespace:System.Reflection.Emit;assembly=mscorlib" x:Class="UdpChatApp.MainWindow"
        Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.196" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF0D4DB5" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <ScrollViewer Height="200" Margin="10">
            <TextBox x:Name="messagesTextBox" IsReadOnly="True" Background="WhiteSmoke" TextWrapping="Wrap" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain"  Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>
-----------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321; 
        private const int targetPort = 12345; 

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;
            
        }

       

        private void ReceiveCallback(IAsyncResult i)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(i, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            MessageLocation($"Received:{message}",false);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void MessageLocation(string message, bool issentme)
        {
            Dispatcher.Invoke(() =>
            {
                var messageItem = new ListBoxItem();
                messageItem.Content = message;

                if (issentme)
                {
                    messageItem.HorizontalContentAlignment = HorizontalAlignment.Right;

                }
                else
                {
                    messageItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                }
                messagesListBox.Items.Add(messageItem);
                messageItem.Focus();
            });

        }



        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                MessageLocation($"Sent:{messageToSend}", true);
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

@@@@@@@@@@@@@

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321; 
        private const int targetPort = 12345; 

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(ar, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            Dispatcher.Invoke(() =>
            {
                DisplayMessage($"Received: {message}", false);
            });
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void DisplayMessage(string message, bool isSentByMe)
        {
            var messageItem = new ListBoxItem();
            messageItem.Content = message;
            messageItem.HorizontalContentAlignment = isSentByMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            messagesListBox.Items.Add(messageItem);
            messagesListBox.ScrollIntoView(messageItem); // Scrolls to the new message
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                DisplayMessage($"Sent: {messageToSend}", true);
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


<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="UdpChatApp.MainWindow"
    Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.196" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF0D4DB5" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain"  Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>

22.05.2024
*********
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321;
        private const int targetPort = 12345;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;

        }



        private void ReceiveCallback(IAsyncResult i)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(i, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            MessageLocation($"Received:{message}", false);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void MessageLocation(string message, bool issentme)
        {
            Dispatcher.Invoke(() =>
            {
                var messageItem = new ListBoxItem();
                messageItem.Content = message;

                if (issentme)
                {
                    messageItem.HorizontalContentAlignment = HorizontalAlignment.Right;

                }
                else
                {
                    messageItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                }
                messagesListBox.Items.Add(messageItem);
                messageItem.Focus();
            });

        }



        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                MessageLocation($"Sent:{messageToSend}", true);
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


----------------------------------------------------

<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="UdpChatApp.MainWindow"
        Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.196" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF0D4DB5" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain"  Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>


22-05-2024

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;//1
using System.IO;//1



namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321;
        private const int targetPort = 12345;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(ar, ref remoteEP);
            string message = Encoding.UTF8.GetString(received);
            Dispatcher.Invoke(() =>
            {
                DisplayMessage($"Received: {message}", false);
            });
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void DisplayMessage(string message, bool isSentMe) 
        {
            var messageItem = new ListBoxItem();
            messageItem.Content = message;
            if (isSentMe)
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
            else
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Left;

            }
            messagesListBox.Items.Add(messageItem);
        
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                DisplayMessage($"Sent: {messageToSend}", true);
                messageTextBox.Clear(); // Clears the message box after sending
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void SelectFileButton_Click(object sender,RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                SendFile(fileName);
            }
        }
        private void SendFile(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIpTextBox.Text), targetPort);
                senderClient.Send(fileBytes,fileBytes.Length, endPoint);
                senderClient.Close();
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

///

<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="UdpChatApp.MainWindow"
    Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.196" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF01050C" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <Button Content="Select File" Margin="10" Click="SelectFileButton_Click" Background="#F4DA"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="#FF040404"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain"  Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>

11111111

<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="UdpChatApp.MainWindow"
    Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.196" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF01050C" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <Button Content="Select File" Margin="10" Click="SelectFileButton_Click" Background="#FF4DA8D8"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain" Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>




using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 54321;
        private const int targetPort = 12345;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(ar, ref remoteEP);
            Dispatcher.Invoke(() =>
            {
                if (IsFile(received))
                {
                    ReceiveFile(received);
                }
                else
                {
                    string message = Encoding.UTF8.GetString(received);
                    DisplayMessage($"Received: {message}", false);
                }
            });
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private bool IsFile(byte[] data)
        {
            // Basit bir dosya kontrolü, veri boyutuna göre ayarlanabilir.
            return data.Length > 100; // 100 byte'tan büyükse dosya olarak kabul et
        }

        private void DisplayMessage(string message, bool isSentMe)
        {
            var messageItem = new ListBoxItem();
            messageItem.Content = message;
            if (isSentMe)
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
            else
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
            messagesListBox.Items.Add(messageItem);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                DisplayMessage($"Sent: {messageToSend}", true);
                messageTextBox.Clear(); // Clears the message box after sending
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                SendFile(fileName);
            }
        }

        private void SendFile(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIpTextBox.Text), targetPort);
                senderClient.Send(fileBytes, fileBytes.Length, endPoint);
                senderClient.Close();
                DisplayMessage($"Sent file: {Path.GetFileName(filePath)}", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending file: {ex.Message}");
            }
        }

        private void ReceiveFile(byte[] fileBytes)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "received_file";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, fileBytes);
                MessageBox.Show($"File received and saved: {saveFileDialog.FileName}");
                DisplayMessage($"Received file: {saveFileDialog.FileName}", false);
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

*******************************************************************

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;



namespace UdpChatApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private const int listenPort = 12345;
        private const int targetPort = 54321;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            udpClient = new UdpClient(listenPort);
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            stbLabelMain.Content = "UDP Client is listening on port " + listenPort;
        }

        private void ReceiveCallback(IAsyncResult i)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
            byte[] received = udpClient.EndReceive(i, ref remoteEP);
            if (received.Length > 0)
            {
                byte[] fileBytes = new byte[received.Length];
                string fileName = "received_file";
                string filePath = System.IO.Path.GetDirectoryName(fileName);

                File.WriteAllBytes(fileName, received);
            }
            else
            {
                string message = Encoding.UTF8.GetString(received);
            }
            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        private void DisplayMessage(string message, bool isSentMe) 
        {
            var messageItem = new ListBoxItem();
            messageItem.Content = message;
            if (isSentMe)
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Right;
            }
            else
            {
                messageItem.HorizontalContentAlignment = HorizontalAlignment.Left;

            }
            messagesListBox.Items.Add(messageItem);
        
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string targetIp = targetIpTextBox.Text;
            string message = messageTextBox.Text;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string messageToSend = $"{time} : {message}";

            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
                byte[] bytesToSend = Encoding.UTF8.GetBytes(messageToSend);
                senderClient.Send(bytesToSend, bytesToSend.Length, endPoint);
                senderClient.Close();

                DisplayMessage($"Sent: {messageToSend}", true);
                messageTextBox.Clear(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void SelectFileButton_Click(object sender,RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                SendFile(fileName);
            }
        }
        private void SendFile(string filePath)
        {            
            byte[] fileBytes = File.ReadAllBytes(filePath);
            try
            {
                UdpClient senderClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIpTextBox.Text), targetPort);
                senderClient.Send(fileBytes,fileBytes.Length, endPoint);
                senderClient.Close();
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



<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    x:Class="UdpChatApp.MainWindow"
    Title="UDP Chat App" Height="450" Width="650">
    <StackPanel Background="#FFE8E8E8">
        <TextBox x:Name="targetIpTextBox" Height="23" Margin="10" Text="172.29.101.190" Foreground="DarkBlue"/>
        <TextBox x:Name="messageTextBox" Height="23" Margin="10" Text="Enter Message" Foreground="#FF01050C" KeyDown="MessageTextBox_KeyDown"/>
        <Button Content="Send Message" Margin="10" Click="SendButton_Click" Background="#FF4DA8D8"/>
        <Button Content="Select File" Margin="10" Click="SelectFileButton_Click" Background="#FF4DA8D8"/>
        <ScrollViewer Height="200" Margin="10">
            <ListBox x:Name="messagesListBox" Background="WhiteSmoke" Foreground="DarkRed"/>
        </ScrollViewer>
        <StatusBar x:Name="stbMain" Height="34">
            <Label x:Name="stbLabelMain"/>
        </StatusBar>
    </StackPanel>
</Window>











