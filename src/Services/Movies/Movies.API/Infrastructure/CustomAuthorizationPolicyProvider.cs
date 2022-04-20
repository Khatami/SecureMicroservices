using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Movies.API.Infrastructure
{
	public class CustomAuthorizationPolicyProvider : IAuthorizationPolicyProvider
	{
		private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

		public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			// ASP.NET Core only uses one authorization policy provider, so if the custom implementation
			// doesn't handle all policies it should fall back to an alternate provider.
			BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
		{
			return Task.FromResult(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser()
				.Build());
		}

		public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
		{
			return Task.FromResult<AuthorizationPolicy>(null);
		}

		public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			if (policyName.StartsWith(CustomAuthorizeAttribute.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
			{
				var scope = policyName.Substring(CustomAuthorizeAttribute.POLICY_PREFIX.Length);

				var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
				policy.RequireClaim("scope", scope);
				return Task.FromResult(policy.Build());
			}

			return BackupPolicyProvider.GetPolicyAsync(policyName);
		}
	}
}
