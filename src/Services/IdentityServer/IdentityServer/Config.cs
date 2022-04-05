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
				ClientId = "movies_mvc_client",
				ClientName = "Movies MVC Web App",
				AllowedGrantTypes = GrantTypes.Code,
				AllowRememberConsent = false, //TODO: ?
				RedirectUris = new List<string>()
				{
					"https://localhost:6700/signin-oidc" // Movies.Client URI
				},
				PostLogoutRedirectUris = new List<string>()
				{
					"https://localhost:6700/signout-callback-oidc" // Movies.Client URI
				},
				ClientSecrets =
				{
					new Secret("secret".Sha256())
				},
				AllowedScopes = new List<string>()
				{
					// REQUIRED. Informs the Authorization Server that the Client is making an OpenID
					// Connect request. If the openid scope value is not present, the behavior is entirely unspecified.
					IdentityServerConstants.StandardScopes.OpenId,

					// OPTIONAL. This scope value requests access to the End-User's default profile
					// Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username,
					// profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
					IdentityServerConstants.StandardScopes.Profile
				}
			}
		};

		public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
		{
		};

		public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
		{
			new ApiScope("movieAPI", "Movie API")
		};

		public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
		{
			new IdentityResources.OpenId(), //TODO: ?
			new IdentityResources.Profile(), //TODO: ?
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
					new Claim(JwtClaimTypes.FamilyName, "Khatami"),
					new Claim(JwtClaimTypes.WebSite, "http://www.hamed.com"),
					new Claim(JwtClaimTypes.Email, "shamedkhatami@gmail.com")
				}
			},

			new TestUser()
			{
				SubjectId = "1A3A8CF9-4533-4442-92CB-1D22E198F6CA",
				Username = "fatemeh",
				Password = "seraj",
				Claims = new List<Claim>()
				{
					new Claim(JwtClaimTypes.GivenName, "Fatemeh"),
					new Claim(JwtClaimTypes.FamilyName, "Seraj"),
					new Claim(JwtClaimTypes.WebSite, "http://www.fatemeh.com"),
					new Claim(JwtClaimTypes.Email, "fatemeh_seraj@outlook.com")
				}
			}
		};
	}
}
