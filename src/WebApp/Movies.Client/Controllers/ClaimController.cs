using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Controllers
{
	public class ClaimController : Controller
	{
		public IActionResult Index()
		{
			if (User.Identity.IsAuthenticated == false)
			{
				return Forbid();
			}

			return View();
		}
	}
}
