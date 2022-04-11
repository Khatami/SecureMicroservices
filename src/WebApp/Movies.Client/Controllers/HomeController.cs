using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Interactive.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
