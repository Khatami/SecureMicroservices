using Microsoft.AspNetCore.Mvc;

namespace Movies.Client.Hybrid.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
