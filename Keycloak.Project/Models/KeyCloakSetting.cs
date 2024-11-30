namespace Keycloak.Project.Models
{
    public class KeyCloakSetting
    {
        public string MetadataAddress { get; set; }
        public string Audience { get; set; }
        public string ValidIssuer { get; set; }
        public string AuthServerUrl { get; set; }
        public string Realm { get; set; }
        public string clientSecret { get; set; }
        public string ClientID { get; set; }
        public string PublicKey { get; set; }
    }
}
