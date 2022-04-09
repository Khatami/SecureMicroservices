using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.Client.HttpHandlers
{
	public class AuthenticationDelegatingHandler : DelegatingHandler
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private ClientCredentialsTokenRequest _clientCredentialsTokenRequest;
		private string _accessToken { get; set; }

		public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory,
			ClientCredentialsTokenRequest clientCredentialsTokenRequest)
		{
			_httpClientFactory = httpClientFactory;
			_clientCredentialsTokenRequest = clientCredentialsTokenRequest;
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

			var httpClient = _httpClientFactory.CreateClient("IDPClient");

			var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_clientCredentialsTokenRequest);
			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}

			_accessToken = tokenResponse.AccessToken;
			request.SetBearerToken(_accessToken);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
