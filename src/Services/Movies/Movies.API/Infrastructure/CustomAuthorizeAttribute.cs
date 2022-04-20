using Microsoft.AspNetCore.Authorization;

namespace Movies.API.Infrastructure
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		public const string POLICY_PREFIX = "APIScope_";

		public CustomAuthorizeAttribute(string apiScope)
		{
			Policy = $"{POLICY_PREFIX}{apiScope}";
		}
	}
}
