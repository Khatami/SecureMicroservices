namespace Movies.Client.Services
{
	public interface IClientCredentialService
	{
		Task<string> GetTokenAsync();
	}
}
