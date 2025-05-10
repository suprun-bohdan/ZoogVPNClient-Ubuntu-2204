using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace vpnClientApp;

public partial class MainWindow : Window
{
    private WindowIcon? _trayIcon;
    private TrayIcon? _tray;
    public MainWindow()
    {
        InitializeComponent();
        this.Opened += OnOpened;
        this.Closing += OnClosing;
        
        this.PropertyChanged += (sender, e) =>
        {
            if (e.Property == Window.WindowStateProperty)
                OnWindowStateChanged(this.WindowState);
        };

    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (_tray == null)
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.png");
            _trayIcon = new WindowIcon(iconPath);
            _tray = new TrayIcon
            {
                Icon = _trayIcon,
                ToolTipText = "VPN Client",
            };
            _tray.Clicked += (s, e) => Dispatcher.UIThread.Post(() =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();
            });
            _tray.IsVisible = true;
        }
    }

    private void OnWindowStateChanged(WindowState state)
    {
        if (state == WindowState.Minimized)
        {
            this.Hide();
        }
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _tray?.Dispose();
    }
}