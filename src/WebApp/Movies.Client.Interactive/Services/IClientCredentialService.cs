namespace Movies.Client.Interactive.Services
{
	public interface IClientCredentialService
	{
		Task<string> GetTokenAsync();
	}
}
