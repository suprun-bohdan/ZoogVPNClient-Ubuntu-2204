using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace vpnClientApp.Services
{
    public class OpenVpnService
    {
        private Process? _vpnProcess;
        private string? _authFilePath;

        public async Task<bool> ConnectAsync(string ovpnPath, string login, string password, Action<string>? onOutput = null)
        {
            if (!File.Exists(ovpnPath))
                throw new FileNotFoundException($"Config not found: {ovpnPath}");

            _authFilePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(_authFilePath, $"{login}\n{password}\n");

            var psi = new ProcessStartInfo
            {
                FileName = "pkexec",
                ArgumentList = { "openvpn", "--config", ovpnPath, "--auth-user-pass", _authFilePath, "--verb", "3" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _vpnProcess = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _vpnProcess.OutputDataReceived += (s, e) => { if (e.Data != null) onOutput?.Invoke(e.Data); };
            _vpnProcess.ErrorDataReceived += (s, e) => { if (e.Data != null) onOutput?.Invoke(e.Data); };

            _vpnProcess.Start();
            _vpnProcess.BeginOutputReadLine();
            _vpnProcess.BeginErrorReadLine();

            return true;
        }

        private void RunCleanupScript()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "pkexec",
                    ArgumentList = { "bash", "-c", "ip link delete tun0; resolvectl revert; ip route replace default via 10.0.2.2 dev enp0s3" },
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch {}
        }

        public void Disconnect()
        {
            if (_vpnProcess != null && !_vpnProcess.HasExited)
            {
                try
                {
                    var killPsi = new ProcessStartInfo
                    {
                        FileName = "pkexec",
                        ArgumentList = { "kill", "-SIGTERM", _vpnProcess.Id.ToString() },
                        UseShellExecute = false
                    };
                    Process.Start(killPsi);
                }
                catch {}
                _vpnProcess.Dispose();
            }
            if (_authFilePath != null && File.Exists(_authFilePath))
            {
                File.Delete(_authFilePath);
            }
            RunCleanupScript();
        }

        ~OpenVpnService()
        {
            RunCleanupScript();
        }
    }
} 