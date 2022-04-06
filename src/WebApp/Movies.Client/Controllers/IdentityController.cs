using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Diagnostics;

namespace Movies.Client.Controllers
{
	[Authorize]
	public class IdentityController : Controller
	{
		public async Task Logout()
		{
			// Signout from the browser
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			// Signout from the OIDC
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
		}

		public async Task<ActionResult> Claims()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return Forbid();
			}

			await LogTokensAndClaims();

			return View();
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
		}
	}
}
