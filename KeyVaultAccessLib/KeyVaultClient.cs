using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace KeyVaultAccessLib
{
    public class KeyVaultClient
    {
        #region Fields
        private readonly SecretClient _client;
        #endregion

        #region Constructor
        public KeyVaultClient(string vaultUrl)
        {
            Uri vaultUri = new Uri(vaultUrl);
            _client = new SecretClient(vaultUri, new DefaultAzureCredential());
        }
        #endregion

        #region Methods
        public async Task<string> GetSecretAsync(string secretName)
        {
            Response<KeyVaultSecret> secret = await _client.GetSecretAsync(secretName);
            return secret.Value.Value;
        }
        #endregion
    }
}
