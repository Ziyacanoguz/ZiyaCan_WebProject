using Microsoft.AspNetCore.Mvc;

namespace ZiyaCan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
