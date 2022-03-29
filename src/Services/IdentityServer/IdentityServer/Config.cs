using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace IdentityServer
{
	public static class Config
	{
		public static IEnumerable<Client> Clients => new Client[]
		{
			// Machine To Machine Communication
			new Client()
			{
				ClientId = "movieClient",
				AllowedGrantTypes = GrantTypes.ClientCredentials,
				ClientSecrets =
				{
					new Secret("secret".Sha256())
				},
				AllowedScopes = { "movieAPI" }
			},

			// Interactive Clients
			new Client()
			{
				ClientId = "movies_mvc_clinet",
				ClientName = "Movies MVC Web App",
				AllowedGrantTypes = GrantTypes.Code,
				AllowRememberConsent = false, //TODO: ?
				RedirectUris = new List<string>()
				{
					"https://localhost:6600/signin-oidc"
				},
				PostLogoutRedirectUris = new List<string>()
				{
					"https://localhost:6600/signout-callback-oidc"
				},
				ClientSecrets =
				{
					new Secret("secret".Sha256())
				}, //TODO: ?
				AllowedScopes = new List<string>()
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile
				}
			}
		};

		public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
		{
			new ApiScope("movieAPI", "Movie API")
		};

		public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
		{
		};

		public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
		{
			//new IdentityResources.OpenId(), //TODO: ?
			//new IdentityResources.Profile() //TODO: ?
		};

		public static List<TestUser> TestUsers => new List<TestUser>
		{
			new TestUser()
			{
				SubjectId = "1A2A8CF9-4533-4442-92CB-1D22E198F6CA",
				Username = "hamed",
				Password = "hamed",
				Claims = new List<Claim>()
				{
					new Claim(JwtClaimTypes.GivenName, "Seyedhamed"),
					new Claim(JwtClaimTypes.FamilyName, "Khatami")
				}
			}
		};
	}
}
