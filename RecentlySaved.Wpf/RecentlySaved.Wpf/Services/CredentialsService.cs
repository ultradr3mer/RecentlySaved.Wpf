using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Services
{
  public class CredentialsService
  {
    #region Fields

    private readonly byte[] entropy = { 8, 6, 2, 4, 9, 3 };
    private string userPassword;

    #endregion Fields

    #region Constructors

    public CredentialsService()
    {
      this.UserName = Properties.Settings.Default.UserName;
      this.userPassword = Properties.Settings.Default.UserPassword;
    }

    #endregion Constructors

    #region Properties

    public bool HasCredentials => !string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.userPassword);

    public string UserName { get; private set; }

    public string UserPassword
    {
      get => this.Decrypt(userPassword);

      private set => userPassword = this.Encrypt(value);
    }

    #endregion Properties

    #region Methods

    public void SetCredentials(string name, string password)
    {
      this.UserName = name;
      this.UserPassword = password;

      Properties.Settings.Default.UserName = this.UserName;
      Properties.Settings.Default.UserPassword = this.userPassword;
      Properties.Settings.Default.Save();
    }

    private string Decrypt(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      byte[] encryptedText = Convert.FromBase64String(text);
      byte[] originalText = ProtectedData.Unprotect(encryptedText, entropy, DataProtectionScope.CurrentUser);
      return Encoding.Unicode.GetString(originalText);
    }

    private string Encrypt(string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return string.Empty;
      }

      byte[] originalText = Encoding.Unicode.GetBytes(text);
      byte[] encryptedText = ProtectedData.Protect(originalText, entropy, DataProtectionScope.CurrentUser);
      return Convert.ToBase64String(encryptedText);
    }

    #endregion Methods
  }
}
