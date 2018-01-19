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
			//var user_roles = MvcApplication.UserRoles;
			//var user_role = (string)user_roles[current_user];

			//if (user_role == "Administrator")
			//{
			//Grant Access
			//}
			//  else
			//  {
			//      return RedirectToAction("Index", "Home");
			//  }
			return View();
		}
	}
}