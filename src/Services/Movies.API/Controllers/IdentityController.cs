using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movies.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class IdentityController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var claims = User.Claims.Select(claim => new { claim.Type, claim.Value });

			return new JsonResult(claims);
		}
	}
}