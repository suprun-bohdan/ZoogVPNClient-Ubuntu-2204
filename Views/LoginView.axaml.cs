using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using vpnClientApp.Services;

namespace vpnClientApp.Views
{
    public partial class LoginView : UserControl
    {
        private readonly CredentialService _credentialService = new();

        public LoginView()
        {
            InitializeComponent();
            this.LoginButton.Click += LoginButton_Click;
            LoadCredentials();
        }

        private async void LoadCredentials()
        {
            var creds = await _credentialService.LoadCredentialsAsync();
            if (creds != null)
            {
                EmailBox.Text = creds.Value.login;
                PasswordBox.Text = creds.Value.password;
                RememberMeBox.IsChecked = true;
            }
        }

        private async void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            if (RememberMeBox.IsChecked == true)
            {
                await _credentialService.SaveCredentialsAsync(EmailBox.Text, PasswordBox.Text);
            }
        }
    }
} 