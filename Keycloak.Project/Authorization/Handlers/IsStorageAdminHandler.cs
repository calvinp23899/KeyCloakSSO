using Keycloak.AuthServices.Common.Claims;
using Keycloak.Project.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace Keycloak.Project.Authorization.Handlers
{
    public class IsStorageAdminHandler : AuthorizationHandler<IsHasAdminRoleRequirement>
    {
        private readonly string _resourceClaim = "resource_access";
        private readonly string _resourceClientId = "datastoreApplication";
        private readonly string _resourceAccess = "StorageAdmin";
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHasAdminRoleRequirement requirement)
        {
            var resourceAccessClaim = context.User.FindFirst(_resourceClaim);
            var rs = context.Resource;
            if (resourceAccessClaim != null && resourceAccessClaim.Type.Equals(_resourceClaim))
            {
                var resourceJson = resourceAccessClaim.Value;

                // The claim value is typically a JSON string, so we need to parse it
                var resourceAccess = JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceJson);
                // Check if the roles contain "PowerAdmin"
                if (resourceAccess != null && resourceAccess.TryGetValue(_resourceClientId, out var access))
                {
                    if (access.Roles.Contains(_resourceAccess))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
