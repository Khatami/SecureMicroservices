using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
