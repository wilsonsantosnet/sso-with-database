using Common.Domain.Base;
using IdentityModel;
using IdentityServer4.Models;
using Newtonsoft.Json;
using Sso.Server.Api.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants;
using System.Linq;

namespace Sso.Server.Api
{
    public class Config
    {
        public static User MakeUsersAdmin()
        {
            return new User
            {
                SubjectId = "1",
                Username = "adm",
                Password = "123456",
                Claims = ClaimsForAdmin("adm", "adm@adm.com.br")
            };
        }

        public static List<Claim> ClaimsForAdmin(string name, string email)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim("role", "admin"),
            };
        }

        public static List<Claim> ClaimsForTenant(int tenantId, string name, string email, int productId, string datasource, string databaseName, int thema)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.Email, email),
                new Claim("role","tenant"),
                new Claim("product",productId.ToString()),
                new Claim("datasource",datasource),
                new Claim("databasename",databaseName),
                new Claim("thema",thema.ToString()),

            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("ssosa", "Sso Basic")
                {
                    Scopes = new List<Scope>()
                    {
                        new Scope
                        {
                            UserClaims = new List<string> {"name", "openid", "email", "role", "tools","product","datasource", "databasename", "thema"},
                            Name = "ssosa",
                            Description = "sso basic",
                        }
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
        }

        public static IEnumerable<Client> GetClients(ConfigSettingsBase settings)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "Score.Platform.Account",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    }
                },

                new Client
                {
                    ClientId = "Score.Platform.Account-spa",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = settings.RedirectUris.ToList(),
                    PostLogoutRedirectUris =  settings.PostLogoutRedirectUris.ToList() ,
                    AllowedCorsOrigins =  settings.ClientAuthorityEndPoint.ToList(),

                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
                new Client
                {
                    ClientId = "hangfire-dash",
                    ClientName = "HangFire Dashboard",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = {
                        "http://localhost:8123/signin-oidc",
                    },

                     AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 43200
                },
                new Client
                {
                    ClientId = "hangfire-api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                        new Secret("segredo".Sha256())
                    },
                    AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    Claims = new List<Claim>{
                        new Claim(JwtClaimTypes.Subject,"991")
                    },
                    AccessTokenLifetime = 43200
                },
                new Client
                {
                    ClientId = "swagger-dash",
                    ClientName = "swagger Dashboard",
                    ClientSecrets = { new Secret("segredo".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = {
                        "http://localhost:8122/swagger/oauth2-redirect.html",
                    },

                     AllowedScopes = {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "ssosa"
                    },
                    RequireConsent = false,
                    AccessTokenLifetime = 43200
                },

            };
        }

        public static List<User> GetUsers()
        {
            return new List<User>()
            {
                MakeUsersAdmin()
            };
        }

    }
}