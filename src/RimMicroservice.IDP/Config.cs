using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace RimMicroservices.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("rim_microservices_api.read", "Rim Microservices API Read Scope"),
            new("rim_microservices_api.write", "Rim Microservices API Write Scope")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("rim_microservices_api", "Rim Microservices API")
            {
                Scopes = new List<string>{ "rim_microservices_api.read" , "rim_microservices_api.write" },
                UserClaims = new List<string>{"role"}
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[] 
        {
            new()
            {
                ClientName = "Rim Microservices Swagger Client",
                ClientId = "rim_microservices_swagger",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                RedirectUris = new List<string>
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html"
                },
                AllowedCorsOrigins = new List<string>
                {
                    "http://localhost:5001"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "rim_microservices_api.read",
                    "rim_microservices_api.write"
                }
            },
            new()
            {
                ClientName = "Rim Microservices Postman Client",
                ClientId = "rim_microservices_postman",
                Enabled = true,
                ClientUri = null,
                RequireClientSecret = true,
                ClientSecrets = new[]
                {
                    new Secret("SuperStrongSecret".Sha512())
                },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword
                },
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    "https://www.getpostman.com/oauth2/callback"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "rim_microservices_api.read",
                    "rim_microservices_api.write"
                }
            }
        };
}