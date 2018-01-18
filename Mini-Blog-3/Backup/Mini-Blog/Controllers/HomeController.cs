using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;


namespace MiniBlog.Controllers
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
			// Replacee all UPPERCASE Variables with custom variables

			var username = Request["username"];
			var password = Request["password"];
			

			if (username == "test" && password == "test")
			{
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://rest.nexmo.com/sms/json");

                var secret = "Scheyss";


				var postData = "api_key=83e6f623";
				postData += "&api_secret=c93bd5200ff97093";
				postData += "&to=41799476139";
				postData += "&from=\"\"NEXMO\"\"";
				postData += "&text=\"" + secret + "\"";
				var data = Encoding.ASCII.GetBytes(postData);

				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = data.Length;

				using (var stream = request.GetRequestStream())
				{
					stream.Write(data, 0, data.Length);
				}

				var response = (HttpWebResponse)request.GetResponse();

				var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

				ViewBag.Message = responseString;

				//return RedirectToAction("Login", "Home");
			}
			else
			{
				ViewBag.Message = "Wrong Credentials";
			}
			return View();
		}
		[HttpPost]
		public void TokenLogin()
		{
            var Token = Request["token"];

            if (Token == "Scheyss")
                {
                ViewBag.Message = "Gratuliere";
                }
            else
			    {
                ViewBag.Message = "Try Again";

				}
           
        }
    }
}
