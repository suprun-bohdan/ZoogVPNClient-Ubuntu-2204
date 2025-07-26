using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace vpnClientApp.Services
{
    public class CredentialService
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "vpnClientApp", "credentials.dat");
        private static readonly byte[] Key = new byte[32] { 21, 42, 63, 84, 105, 126, 147, 168, 189, 210, 231, 252, 17, 34, 51, 68, 85, 102, 119, 136, 153, 170, 187, 204, 221, 238, 255, 1, 2, 3, 4, 5 };
        private static readonly byte[] IV = new byte[16] { 11, 22, 33, 44, 55, 66, 77, 88, 99, 110, 121, 132, 143, 154, 165, 176 };

        public async Task SaveCredentialsAsync(string login, string password)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var plainText = $"{login}\n{password}";
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                await sw.WriteAsync(plainText);
            var encrypted = ms.ToArray();
            await File.WriteAllBytesAsync(FilePath, encrypted);
        }

        public async Task<(string login, string password)?> LoadCredentialsAsync()
        {
            if (!File.Exists(FilePath)) return null;
            var encrypted = await File.ReadAllBytesAsync(FilePath);
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(encrypted);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            var plainText = await sr.ReadToEndAsync();
            var parts = plainText.Split('\n');
            if (parts.Length != 2) return null;
            return (parts[0], parts[1]);
        }
    }
} 