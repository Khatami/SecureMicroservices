using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.Client.Hybrid.HttpHandlers
{
	public class AuthenticationDelegatingHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private string _accessToken { get; set; }
		public AuthenticationDelegatingHandler(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(_accessToken) == false)
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtSecurityToken = tokenHandler.ReadJwtToken(_accessToken);

				if (jwtSecurityToken.ValidTo > DateTime.UtcNow.AddSeconds(60))
				{
					request.SetBearerToken(_accessToken);

					return await base.SendAsync(request, cancellationToken);
				}
			}

			_accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

			request.SetBearerToken(_accessToken);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
