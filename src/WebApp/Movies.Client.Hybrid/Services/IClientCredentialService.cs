namespace Movies.Client.Hybrid.Services
{
	public interface IClientCredentialService
	{
		Task<string> GetTokenAsync();
	}
}
