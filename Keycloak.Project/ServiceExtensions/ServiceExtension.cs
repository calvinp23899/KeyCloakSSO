using Keycloak.AuthServices.Authorization;
using Keycloak.Project.Authorization.Handlers;
using Keycloak.Project.Authorization.Requirements;
using Keycloak.Project.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;

namespace Keycloak.Project.ServiceExtensions
{
    public static class ServiceExtension
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //Example 1:
            var keycloakSettings = configuration.GetSection("Keycloak").Get<KeyCloakSetting>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                string pemPublicKey = $@"
                    -----BEGIN PUBLIC KEY-----
                    {keycloakSettings.PublicKey}  
                    -----END PUBLIC KEY-----";
                RSA rsa = RSA.Create();
                rsa.ImportFromPem(pemPublicKey.ToCharArray());
                //options.MetadataAddress = keycloakSettings.MetadataAddress;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = keycloakSettings.ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = keycloakSettings.Audience, // Use an array for multiple audiences
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new RsaSecurityKey(rsa), // Use the RSA public key for validation
                    ValidateIssuerSigningKey = true,

                };
            });
        }

        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                       new OpenApiSecurityScheme
                       {
                          Reference = new OpenApiReference
                          {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                          }
                       },
                       Array.Empty<string>()
                    }
                });
            });
        }

        public static void ConfigureAuthorize(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, IsStorageAdminHandler>();
            services.AddKeycloakAuthorization(options =>
            {
                options.EnableRolesMapping =
                    RolesClaimTransformationSource.ResourceAccess;
                options.RolesResource = "test-data2";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", builder =>
                {
                    builder.AddRequirements(new IsHasAdminRoleRequirement());
                    builder.RequireAuthenticatedUser(); // work well

                });
            });
        }
    }
}
