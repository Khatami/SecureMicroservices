using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Movies.Client.Hybrid.HttpHandlers
{
	public class AuthenticationDelegatingHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _contextAccessor;
		public AuthenticationDelegatingHandler(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

			request.SetBearerToken(accessToken);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
