<Window x:Class="AdvancedUdpComms.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="UDP Haberleşme Uygulaması" Height="350" Width="525">
    <Grid>
        <ListBox x:Name="UserListBox" HorizontalAlignment="Left" Height="100" Margin="10,10,0,0" VerticalAlignment="Top" Width="120"/>
        <TextBox x:Name="MessageTextBox" HorizontalAlignment="Left" Height="23" Margin="10,120,0,0" VerticalAlignment="Top" Width="120"/>
        <Button Content="Gönder" HorizontalAlignment="Left" Margin="10,150,0,0" VerticalAlignment="Top" Width="75" Click="SendButton_Click"/>
        <TextBox x:Name="MessagesTextBox" HorizontalAlignment="Left" Height="100" Margin="140,10,0,0" VerticalAlignment="Top" Width="355" IsReadOnly="True"/>
    </Grid>
</Window>

