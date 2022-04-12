using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Interactive.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
