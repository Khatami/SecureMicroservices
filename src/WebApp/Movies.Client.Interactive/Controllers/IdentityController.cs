using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;

namespace Movies.Client.Interactive.Controllers
{
	[Authorize]
	public class IdentityController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public IdentityController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		[AllowAnonymous]
		public IActionResult Login()
		{
			// A ChallengeResult is an ActionResult that when executed, challenges the given authentication schemes' handler
			// A challenge is basically a way of saying "I don't know who this user is, please verify their identity".
			// So if the authentication handler triggered is e.g. the Facebook authentication handler,
			// it will react to the challenge by issuing a redirect to the Facebook authentication page.
			// A local account authentication handler might issue a redirect to the local sign-in page.

			// In the case of JWT Bearer authentication, the handler cannot do anything other than respond with a 401 status code and leave it up to the caller to authenticate themselves properly.
			// var challenge = Challenge(new AuthenticationProperties

			return Challenge(new AuthenticationProperties
			{
				RedirectUri = "/"
			});
		}

		public async Task Logout()
		{
			// Signout from the browser
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			// Signout from the OIDC
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
		}

		public async Task<ActionResult> UserInfo()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return Forbid();
			}

			var client = _httpClientFactory.CreateClient("IDClient");
			var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
			if (discoveryDocumentResponse.IsError)
			{
				return null;
			}

			var userInfo = await client.GetUserInfoAsync(new UserInfoRequest()
			{
				Address = discoveryDocumentResponse.UserInfoEndpoint,
				Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
			});

			await LogTokensAndClaims();

			return View(userInfo);
		}

		public async Task LogTokensAndClaims()
		{
			var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

			Debug.WriteLine($"Identity token: {identityToken}");

			var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

			Debug.WriteLine($"Access token: {accessToken}");

			foreach (var claim in User.Claims)
			{
				Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
			}

			// cookies decryption (for testing)

			// ONE - grab the CookieAuthenticationOptions instance
			var cookieAuthenticationOptions = HttpContext.RequestServices
				.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
				.Get(CookieAuthenticationDefaults.AuthenticationScheme); //or use .Get("Cookies")

			// TWO - Get the encrypted cookie value
			var cookie = cookieAuthenticationOptions.CookieManager
				.GetRequestCookie(HttpContext, cookieAuthenticationOptions.Cookie.Name);

			// THREE - decrypt it
			var unprotectedCookie = cookieAuthenticationOptions.TicketDataFormat.Unprotect(cookie);
		}
	}
}
