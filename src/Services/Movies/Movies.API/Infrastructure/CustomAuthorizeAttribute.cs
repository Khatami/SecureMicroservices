using Microsoft.AspNetCore.Authorization;

namespace Movies.API.Infrastructure
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		public CustomAuthorizeAttribute(string apiScope)
		{
			Policy = apiScope;
		}
	}
}
