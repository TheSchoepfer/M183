using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace HTTP_Digest_Authentication.Controllers
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

        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];
            var stay_logged_in = Request["stay_logged_in"];

            if (username == "test" && password == "test")
            {
                
                if(stay_logged_in == "on")
                {
					var auth_cookie = new HttpCookie("LoginCookie");
					auth_cookie.Value = "Test";
					auth_cookie.Expires = DateTime.Now.AddDays(14d);
					auth_cookie.Path = "localhost:8080";

					Response.Cookies.Add(auth_cookie);
                }

                else
                {
                    Session["LoginCookie"] = "Test";
                }
				Response.Redirect("Admin/Home");
            }

            else
            {
                ViewBag.content = "Failed to log in!";
            }
            return View();
        }
    }


}
