using IdentityModel.Client;
using Movies.Client.Configuration;

namespace Movies.Client.Services
{
	public class ClientCredentialService : IClientCredentialService
	{
		private readonly IConfiguration _configuration;

		public ClientCredentialService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<string> GetTokenAsync()
		{
			var clientCredential = _configuration.GetSection("ClientCredential").Get<ClientCredential>();

			var httpClient = new HttpClient();
			var discoveryDocument = await httpClient.GetDiscoveryDocumentAsync(clientCredential.Address);

			if (discoveryDocument.IsError)
			{
				throw new Exception(discoveryDocument.Error);
			}

			var apiClientCredentials = new ClientCredentialsTokenRequest()
			{
				Address = discoveryDocument.TokenEndpoint,
				ClientId = clientCredential.ClientId,
				ClientSecret = clientCredential.ClientSecret,
				Scope = clientCredential.Scope
			};

			var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(apiClientCredentials);
			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}

			return tokenResponse.AccessToken;
		}
	}
}
