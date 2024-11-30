using Keycloak.Project.Models;

namespace Keycloak.Project.Interfaces
{
    public interface IKeyCloakClient
    {
        Task<string> GetTokenKeyCloak(LoginDto request);

    }
}
