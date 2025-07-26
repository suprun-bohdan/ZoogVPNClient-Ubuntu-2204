using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace vpnClientApp.Services
{
    public class OvpnConfigItem
    {
        public string DisplayName { get; set; } = "";
        public string FullPath { get; set; } = "";
        public override string ToString() => DisplayName;
    }

    public class OvpnConfigService
    {
        public List<OvpnConfigItem> GetOvpnFiles(string directory)
        {
            if (!Directory.Exists(directory)) return new List<OvpnConfigItem>();
            return Directory.GetFiles(directory, "*.ovpn", SearchOption.TopDirectoryOnly)
                .Select(f => new OvpnConfigItem
                {
                    FullPath = f,
                    DisplayName = ParseCountryName(Path.GetFileNameWithoutExtension(f))
                })
                .ToList();
        }

        private string ParseCountryName(string fileName)
        {
            var match = Regex.Match(fileName, @"^([^\-\(]+)");
            var country = match.Success
                ? match.Groups[1].Value.Trim()
                : fileName;

            var protocol = fileName.ToLower().Contains("udp") ? "UDP"
                        : fileName.ToLower().Contains("tcp") ? "TCP"
                        : null;

            return protocol != null ? $"{country} ({protocol})" : country;
        }
    }
} 