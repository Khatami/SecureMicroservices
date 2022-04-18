using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer
{
	public class IdentityProfileService : IProfileService
	{
		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			var claimsNames = new List<string>();
			claimsNames.AddRange(context.RequestedResources.Resources.IdentityResources.SelectMany(r => r.UserClaims));
			claimsNames.AddRange(context.RequestedResources.Resources.ApiResources.SelectMany(r => r.UserClaims));
			claimsNames.AddRange(context.RequestedResources.Resources.ApiScopes.SelectMany(r => r.UserClaims));
			context.RequestedClaimTypes = claimsNames;

			var sub = context.Subject.GetSubjectId();
			var user = Config.TestUsers.First(q => q.SubjectId == sub);
			if (user == null)
			{
				throw new ArgumentException("");
			}

			var claims = user.Claims
				.Where(q => context.RequestedClaimTypes.Contains(q.Type))
				.ToList();

			context.IssuedClaims = claims;
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			var sub = context.Subject.GetSubjectId();
			var user = Config.TestUsers.First(q => q.SubjectId == sub);
			context.IsActive = user != null;
		}
	}
}
