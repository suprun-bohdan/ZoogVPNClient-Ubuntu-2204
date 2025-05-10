# VPN Client App

A modern VPN client application built with C#, .NET 8, and Avalonia UI, designed for Ubuntu 22.04.

## Features

- Clean, modern user interface
- OpenVPN integration with config file support
- Secure credential storage with encryption
- System tray integration
- Real-time connection status and logs
- Server selection from OpenVPN configs
- Automatic privilege escalation for OpenVPN process

## Project Structure

```
vpnClientApp/
├── Configs/           # OpenVPN configuration files
├── Services/          # Business logic and services
├── Views/            # UI components
├── App.axaml         # Main application XAML
├── MainWindow.axaml  # Main window XAML
└── Program.cs        # Application entry point
```

## Requirements

- .NET 8 Runtime
- OpenVPN
- Ubuntu 22.04 (for .deb package)

## Development Setup

1. Clone the repository
2. Install .NET 8 SDK
3. Install required packages:
```bash
dotnet restore
```

4. Build the project:
```bash
dotnet build
```

5. Run the application:
```bash
dotnet run
```

## Building .deb Package

Use the provided `build_deb.sh` script to create a .deb package:

```bash
./build_deb.sh
```

This will:
1. Publish the application
2. Create necessary directory structure
3. Copy required files
4. Build the .deb package

## Installation

Install the .deb package:

```bash
sudo dpkg -i vpnClientApp_1.0.0.deb
sudo apt-get install -f
```

## Usage

1. Launch the application
2. Enter your VPN credentials
3. Select a server from the dropdown
4. Click "Connect" to establish the VPN connection
5. The application will minimize to the system tray when closed
6. Right-click the tray icon to access the menu

## Security Features

- Credentials are stored encrypted in a local file
- OpenVPN process runs with elevated privileges using pkexec
- No sensitive data is stored in plain text
- Secure handling of OpenVPN configurations

## Dependencies

- Avalonia UI
- .NET 8
- OpenVPN
- System tray integration libraries

## License

This project is licensed under the MIT License. 