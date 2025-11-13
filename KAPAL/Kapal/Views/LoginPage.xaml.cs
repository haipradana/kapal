using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kapal.Views
{
    public partial class LoginPage : Page
    {
        private readonly Func<bool, Task> _onLoginSuccess;
        private string _adminUsername = "admin";
        private string _adminPassword = "password";

        public LoginPage(Func<bool, Task> onLoginSuccess)
        {
     InitializeComponent();
   _onLoginSuccess = onLoginSuccess;
     LoadCredentialsFromEnv();
      
     // Focus on username field
        Loaded += (_, __) => txtUsername.Focus();
    
   // Enter key support
        txtPassword.KeyDown += (s, e) =>
  {
          if (e.Key == System.Windows.Input.Key.Enter)
      BtnLogin_Click(null, null);
     };
    }

 private void LoadCredentialsFromEnv()
        {
 try
    {
      // Cari file .env di root project
    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
       var envPath = Path.Combine(baseDir, ".env");
                
         // Fallback: cari di parent directories
     if (!File.Exists(envPath))
   {
  var parentDir = Directory.GetParent(baseDir);
      for (int i = 0; i < 5 && parentDir != null; i++)
        {
          envPath = Path.Combine(parentDir.FullName, ".env");
   if (File.Exists(envPath)) break;
       parentDir = parentDir.Parent;
         }
 }

   if (File.Exists(envPath))
      {
   var lines = File.ReadAllLines(envPath);
    foreach (var line in lines)
    {
   if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
          continue;

    var parts = line.Split('=', 2);
  if (parts.Length == 2)
             {
      var key = parts[0].Trim();
            var value = parts[1].Trim();

        if (key == "ADMIN_USERNAME")
          _adminUsername = value;
      else if (key == "ADMIN_PASSWORD")
     _adminPassword = value;
}
   }
    }
        }
     catch (Exception ex)
  {
          // Silent fail, use default credentials
      System.Diagnostics.Debug.WriteLine($"Error loading .env: {ex.Message}");
  }
     }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
      {
      var username = txtUsername.Text.Trim();
  var password = txtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
{
      MessageBox.Show("Username dan password harus diisi.", "Login Failed",
        MessageBoxButton.OK, MessageBoxImage.Warning);
    return;
  }

       if (username == _adminUsername && password == _adminPassword)
 {
       // Disable login controls during initialization
      btnLogin.IsEnabled = false;
        btnGuest.IsEnabled = false;
       txtUsername.IsEnabled = false;
       txtPassword.IsEnabled = false;
     
  await _onLoginSuccess?.Invoke(true); // true = admin
        }
 else
     {
                MessageBox.Show("Username atau password salah.", "Login Failed",
         MessageBoxButton.OK, MessageBoxImage.Error);
          }
        }

      private async void BtnGuest_Click(object sender, RoutedEventArgs e)
 {
            // Disable login controls during initialization
       btnLogin.IsEnabled = false;
    btnGuest.IsEnabled = false;
 txtUsername.IsEnabled = false;
      txtPassword.IsEnabled = false;
  
   await _onLoginSuccess?.Invoke(false); // false = guest
        }
    }
}
