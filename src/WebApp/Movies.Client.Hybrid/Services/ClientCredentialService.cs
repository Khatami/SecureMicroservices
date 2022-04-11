using IdentityModel.Client;
using Movies.Client.Hybrid.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.Client.Hybrid.Services
{
	public class ClientCredentialService : IClientCredentialService
	{
		private readonly IConfiguration _configuration;
		private string _accessToken { get; set; }

		public ClientCredentialService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<string> GetTokenAsync()
		{
			if (string.IsNullOrEmpty(_accessToken) == false)
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var jwtSecurityToken = tokenHandler.ReadJwtToken(_accessToken);

				if (jwtSecurityToken.ValidTo > DateTime.UtcNow.AddSeconds(60))
				{
					return _accessToken;
				}
			}

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

			_accessToken = tokenResponse.AccessToken;
			return _accessToken;
		}
	}
}
