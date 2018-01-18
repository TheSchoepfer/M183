using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
		public ActionResult UserDashboard()
		{
            var Token = Request["token"];

            if (Token == "Scheyss")
                {
                ViewBag.Message = "Gratuliere";
                ViewBag.SuccessMessage = "Got it";
                }
            else
			    {
                ViewBag.Message = "Try Again";
                //'sadasd'
				}
			return View();
		}
        /// <summary>
        /// Die Posts des bestimmten Users laden
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDashboard4()
        {
            //DB connection
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\vognstrl\source\repos\M183\Mini-Blog\Mini-Blog\Database\miniBlogDB.mdf;Integrated Security=True;Connect Timeout=30";
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            
            int currentUserId = 0; //Get current UserId

            //Get All Posts where Post.user_id == User.Id
            cmd.CommandText = "SELECT title ,description ,content ,createdon ,modifiedon FROM dbo.Post WHERE user_id=" + currentUserId.ToString();
            reader = cmd.ExecuteReader();
            List<Models.Post> Posts = new List<Models.Post>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Posts.Add(new Models.Post { Title = reader.GetString(1), Content = reader.GetString(3), Description = reader.GetString(2), Id = reader.GetInt32(0), Createdon = reader.GetDateTime(4), Modifiedon = reader.GetDateTime(5) });
                }
            }
            else
            {
                //Meldung oder Ähnliches abgeben dass noch kein Post vorhanden ist. --> NICE TO HAVE
            }

            //Create new Model, and fill in db informations
            Models.DashboardModel ViewDashboardModel = new Models.DashboardModel();
            ViewDashboardModel.Postlist = Posts;

            //Send Model to UserDashboardView
            return View();
        }

    }
}
