using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Lab_Role_Based_Authorization.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var mvcName = typeof(Controller).Assembly.GetName();
            var isMono = Type.GetType("Mono.Runtime") != null;

            ViewData["Version"] = mvcName.Version.Major + "." + mvcName.Version.Minor;
            ViewData["Runtime"] = isMono ? "Mono" : ".NET";

            return View();
        }
        [HttpPost]
		public ActionResult Login()
		{
            var username = Request["username"];
            var password = Request["password"];

            if(username == "admin" && password == "test")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if(username == "user" && password == "test")
            {
                return RedirectToAction("Dashboard", "User");
            }
            else
            {
                ViewBag.Message = "Kein Korrektes Passwort";
            }

			return View();
		}
    }
}
