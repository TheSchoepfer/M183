using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBlog.Controllers
{
	public class UserController : Controller
	{
		// Admin
		public ActionResult Dashboard()
		{
			var current_user = (string)Session["username"];
			return RedirectToAction("Index", "Home");
			//  }
			return View();
		}
	}
}