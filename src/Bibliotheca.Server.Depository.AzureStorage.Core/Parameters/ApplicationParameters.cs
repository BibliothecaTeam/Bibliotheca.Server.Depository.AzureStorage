namespace Bibliotheca.Server.Depository.AzureStorage.Core.Parameters
{
    public class ApplicationParameters
    {
        public string SecureToken { get; set; }

        public string OAuthAuthority { get; set; }

        public string OAuthAudience { get; set; }

        public string AzureStorageConnectionString { get; set; }

        public ServiceDiscovery ServiceDiscovery { get; set; }
    }
}