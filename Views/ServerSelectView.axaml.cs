using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;
using vpnClientApp.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace vpnClientApp.Views
{
    public partial class ServerSelectView : UserControl
    {
        private readonly OvpnConfigService _ovpnService = new();
        private readonly OpenVpnService _vpnService = new();
        private ObservableCollection<OvpnConfigItem> _allFiles = new();
        public OvpnConfigItem? SelectedItem => ServerCombo.SelectedItem as OvpnConfigItem;
        private bool _isConnected = false;

        public ServerSelectView()
        {
            InitializeComponent();
            ServerCombo.ItemsSource = _allFiles;
            LoadServers();
            ConnectButton.Click += ConnectButton_Click;
        }

        private void LoadServers()
        {
            var dir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Configs");
            System.Console.WriteLine($"[VPN CLIENT] Looking for configs in: {dir}");
            var files = _ovpnService.GetOvpnFiles(dir).OrderBy(f => f.DisplayName).ToList();
            System.Console.WriteLine($"[VPN CLIENT] Found {files.Count} .ovpn files");
            _allFiles.Clear();
            foreach (var f in files)
                _allFiles.Add(f);
            ServerCombo.ItemsSource = null;
            ServerCombo.ItemsSource = _allFiles;
        }

        private async void ConnectButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_isConnected)
            {
                _vpnService.Disconnect();
                StatusBlock.Text = "Disconnected";
                ConnectButton.Content = "Connect";
                _isConnected = false;
                return;
            }
            LogBox.Text = string.Empty;
            if (SelectedItem == null)
            {
                StatusBlock.Text = "Select server";
                return;
            }
            var login = UserNameBox.Text ?? string.Empty;
            var password = PasswordBox.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                StatusBlock.Text = "Enter login and password";
                return;
            }
            StatusBlock.Text = "Connecting...";
            ConnectButton.Content = "Connecting...";
            try 
            {
                await Task.Run(() => _vpnService.ConnectAsync(SelectedItem.FullPath, login, password, (log) => 
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        LogBox.Text += log + "\n";
                        LogBox.CaretIndex = LogBox.Text.Length;
                        if (log.Contains("Initialization Sequence Completed"))
                        {
                            StatusBlock.Text = "Connected";
                            ConnectButton.Content = "Disconnect";
                            _isConnected = true;
                        }
                        else if (log.Contains("AUTH_FAILED"))
                        {
                            StatusBlock.Text = "Authentication failed";
                            ConnectButton.Content = "Connect";
                            _isConnected = false;
                        }
                    });
                }));
            }
            catch (System.Exception ex)
            {
                StatusBlock.Text = $"Error: {ex.Message}";
                ConnectButton.Content = "Connect";
                _isConnected = false;
            }
        }
    }
} 