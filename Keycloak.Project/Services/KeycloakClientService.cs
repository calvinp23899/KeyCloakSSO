using Keycloak.Project.Interfaces;
using Keycloak.Project.Models;

namespace Keycloak.Project.Services
{
    public class KeycloakClientService : IKeyCloakClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public KeycloakClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetTokenKeyCloak(LoginDto request)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            var keycloakUrl = $"{_configuration["KeyCloak:AuthServerUrl"]}realms/{_configuration["KeyCloak:Realm"]}/protocol/openid-connect/token";
            var clientId = _configuration["KeyCloak:ClientId"];
            var clientSecret = _configuration["KeyCloak:ClientSecret"];
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", $"{request.Username}"),
                new KeyValuePair<string, string>("password", $"{request.Password}"),
                new KeyValuePair<string, string>("client_id", $"{clientId}"),
                new KeyValuePair<string, string>("client_secret", $"{clientSecret}")
            });
            var response = await _httpClient.PostAsync(keycloakUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            return null;
        }
    }
}
