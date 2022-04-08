using IdentityModel.Client;

namespace Movies.Client.HttpHandlers
{
	public class AuthenticationDelegatingHandler : DelegatingHandler
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private ClientCredentialsTokenRequest _clientCredentialsTokenRequest;

		public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory,
			ClientCredentialsTokenRequest clientCredentialsTokenRequest)
		{
			_httpClientFactory = httpClientFactory;
			_clientCredentialsTokenRequest = clientCredentialsTokenRequest;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var httpClient = _httpClientFactory.CreateClient("IDPClient");

			var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_clientCredentialsTokenRequest);
			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}

			request.SetBearerToken(tokenResponse.AccessToken);

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
