﻿using IdentityModel;
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
				AllowedScopes = 
				{
					/*
					   ************************************************************
					   ApiScopes
					   ************************************************************
					*/

					"movieAPI"

					/*
					   ************************************************************
					   /ApiScopes
					   ************************************************************
					*/
				}
			},

			// Interactive Clients
			new Client()
			{
				ClientId = "movies_mvc_client_interactive",
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
					/*
					   ************************************************************
					   IdentityResources
					   ************************************************************
					*/

					// REQUIRED. Informs the Authorization Server that the Client is making an OpenID
					// Connect request. If the openid scope value is not present, the behavior is entirely unspecified.
					IdentityServerConstants.StandardScopes.OpenId,

					// OPTIONAL. This scope value requests access to the End-User's default profile
					// Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username,
					// profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
					IdentityServerConstants.StandardScopes.Profile,

					"AllowedServices",

					/*
					   ************************************************************
					   /IdentityResources
					   ************************************************************
					*/

					/*
					   ************************************************************
					   ApiScopes
					   ************************************************************
					*/

					"movieAPI"

					/*
					   ************************************************************
					   /ApiScopes
					   ************************************************************
					*/
				}
			},

			// Hybrid Clients
			new Client()
			{
				ClientId = "movies_mvc_client_hybrid",
				ClientName = "Movies MVC Web App",
				AllowedGrantTypes = GrantTypes.Hybrid,

				// Proof Key for Code Exchange (abbreviated PKCE, pronounced “pixie”)
				// is an extension to the authorization code flow to prevent CSRF and authorization code injection attacks.
				RequirePkce = false, //TODO: ?
				AllowRememberConsent = false, //TODO: ?
				RedirectUris = new List<string>()
				{
					"https://localhost:6600/signin-oidc" // Movies.Client URI
				},
				PostLogoutRedirectUris = new List<string>()
				{
					"https://localhost:6600/signout-callback-oidc" // Movies.Client URI
				},
				ClientSecrets =
				{
					new Secret("secret".Sha256())
				},
				AllowedScopes = new List<string>()
				{
					/*
					   ************************************************************
					   IdentityResources
					   ************************************************************
					*/

					// REQUIRED. Informs the Authorization Server that the Client is making an OpenID
					// Connect request. If the openid scope value is not present, the behavior is entirely unspecified.
					IdentityServerConstants.StandardScopes.OpenId,

					// OPTIONAL. This scope value requests access to the End-User's default profile
					// Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username,
					// profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
					IdentityServerConstants.StandardScopes.Profile,

					"AllowedServices",

					/*
					   ************************************************************
					   /IdentityResources
					   ************************************************************
					*/

					/*
					   ************************************************************
					   ApiScopes
					   ************************************************************
					*/

					"movieAPI"

					/*
					   ************************************************************
					   /ApiScopes
					   ************************************************************
					*/
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

		// An identity resource is a named group of claims that can be requested using the scope parameter.
		// Once the resource is defined, you can give access to it to a client via the AllowedScopes option
		public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
		{
			/*
			   ************************************************************
			   openid
			   ************************************************************
			*/

			//new IdentityResource(
			//	name: "openid",
			//	userClaims: new[] { "sub" },
			//	displayName: "Your user identifier");

			// sub => SubjectID
			new IdentityResources.OpenId(),

			/*
			   ************************************************************
			   /openid
			   ************************************************************
			*/

			/*
			   ************************************************************
			   profile
			   ************************************************************
			*/
			new IdentityResource(
				name: "profile",
				userClaims: new[]
				{
					JwtClaimTypes.GivenName,
					JwtClaimTypes.FamilyName,
					JwtClaimTypes.WebSite,
					JwtClaimTypes.Email,
				},
				displayName: "Your profile data"),

			//new IdentityResources.Profile(),
			
			/*
			   ************************************************************
			   /profile
			   ************************************************************
			*/

			/*
			   ************************************************************
			   custom identity resources
			   ************************************************************
			*/
			new IdentityResource(
				name: "AllowedServices",
				userClaims: new[] { "services" },
				displayName: "Services, which are allowed to access")

			/*
			   ************************************************************
			   /custom identity resources
			   ************************************************************
			*/
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
					new Claim(JwtClaimTypes.Email, "shamedkhatami@gmail.com"),
					new Claim("services", "A,B,C")
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
					new Claim(JwtClaimTypes.Email, "fatemeh_seraj@outlook.com"),
					new Claim("services", "A,B")
				}
			}
		};
	}
}
