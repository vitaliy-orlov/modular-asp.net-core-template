using Microsoft.AspNetCore.Mvc;
using Module.Account.Models;
using Core;

namespace Module.Account.Controllers
{
    [Area(ModuleInfo.AREA_NAME)]
    [MenuTitle("Profile")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new AccountInfo() { Login = "admin", FirstName = "<no name>" });
        }
    }
}
