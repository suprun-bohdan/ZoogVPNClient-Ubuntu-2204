using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace vpnClientApp.Services;

public class CredentialsService
{
    private const string CredentialsFile = "credentials.dat";
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("YourSecretKey1234567890123456789012");
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456");

    public class StoredCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public async Task SaveCredentialsAsync(string username, string password)
    {
        var credentials = new StoredCredentials
        {
            Username = username,
            Password = password,
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        var json = JsonSerializer.Serialize(credentials);
        var encryptedData = EncryptString(json);
        await File.WriteAllBytesAsync(CredentialsFile, encryptedData);
    }

    public async Task<(string username, string password)?> LoadCredentialsAsync()
    {
        if (!File.Exists(CredentialsFile))
            return null;

        var encryptedData = await File.ReadAllBytesAsync(CredentialsFile);
        var json = DecryptString(encryptedData);
        var credentials = JsonSerializer.Deserialize<StoredCredentials>(json);

        if (credentials.ExpirationDate < DateTime.Now)
        {
            File.Delete(CredentialsFile);
            return null;
        }

        return (credentials.Username, credentials.Password);
    }

    public void ClearCredentials()
    {
        if (File.Exists(CredentialsFile))
            File.Delete(CredentialsFile);
    }

    private static byte[] EncryptString(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return msEncrypt.ToArray();
    }

    private static string DecryptString(byte[] cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
} 