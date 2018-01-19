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
            var username = Request["username"];
            var password = Request["password"];
            Random random = new Random();
            int token = random.Next(1001, 9999);

            //SQL DB abfrage
            string ConnectionString = @"Data Source = (localdb)\MSSQLLocalDB ; AttachDbFilename = /Users/schoepfer/Documents/GitHub/M183/M183/Mini-Blog/Mini-Blog/Database/miniBlogDB; Integrated Security = True; Connect Timeout = 30";
            SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();

            SqlCommand command = new SqlCommand();
            SqlDataReader reader;

            //Sucht nach Benutzern mit den eingegebenen Daten
            command.CommandText = "SELECT [Id], [Username], [Password], [Role] FROM [dbo].[User] WHERE [Username] = '" + username + "' AND [Password] = '" + password + "'";
            command.Connection = connection;

            reader = command.ExecuteReader();

            int ID = new int();
            int UserId = new int();
            string Username = String.Empty;
            string Password = String.Empty;
            string Role = String.Empty;


            if (reader.HasRows)
            {
                //Die Werte, welche in der Datenbank stehen auslesen
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader["Id"]);
                    Username = Convert.ToString(reader["Username"]);
                    Password = Convert.ToString(reader["Password"]);
                    Role = Convert.ToString(reader["Role"]);
                }
                var request = (HttpWebRequest)WebRequest.Create("https://rest.nexmo.com/sms/json");

                //Session starten
                Session["SessionUsername"] = Username;
                Session["SessionPassword"] = Password;
                var secretToken = Convert.ToString(token);
                DateTime Time = DateTime.Now;
                string TimeDate = Time.ToString();

                //Das Token abspeichern
                command.CommandText = "INSERT INTO [dbo].[Token] ([UserId], [Token], [Expiry], [DeletedOn] ) VALUES (" + ID + ", '" + secretToken + "', '" + TimeDate + "', 0 )";
                //command.ExecuteNonQuery();

                //Das Token wird via SMS an den Empfänger versendet
                var postData = "api_key=83e6f623";
                postData += "&api_secret=25a086b15fa31600";
                postData += "&to=41799476139";
                postData += "&from=\"\"NEXMO\"\"";
                postData += "&text=\"" + secretToken + "\"";
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

                return RedirectToAction("LoginToken", "Home");
            }
            else
            {
                ViewBag.Message = "Wrong Credentials";
            }

            //Das Admin Login
            if (username == "admin" && password == "test")
            {
                Session["username"] = "admin";

                return RedirectToAction("Dashboard", "Admin");
            }
            else if (username == "user" && password == "test")
            {

            }

            return View();
        }

        public ActionResult TokenLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckofToken()
        {
            var username = Session["SessionUsername"].ToString();
            var password = Session["SessionPassword"].ToString();
            var secretToken = Request["token"];

            string ConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;AttachDbFilename=/Users/schoepfer/Documents/GitHub/M183/M183/Mini-Blog/Mini-Blog/Database/miniBlogDB; Integrated Security = True; Connect Timeout = 30";
            SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();

            SqlCommand command = new SqlCommand();
            SqlDataReader reader;

            command.CommandText = "SELECT [Id] FROM [dbo].[User] WHERE [Username] = '" + username + "' AND [Password] = '" + password + "'";
            command.Connection = connection;
            reader = command.ExecuteReader();

            int ID = new int();


            if (reader.HasRows)
            {
                //Get the Values
                while (reader.Read())
                {
                    ID = Convert.ToInt32(reader["Id"]);
                }
            }
            if (reader != null)
            {
                reader.Close();
            }

            command = new SqlCommand();
            command.CommandText = "SELECT [Token], [Expiry] FROM [dbo].[Token] WHERE [Id] = " + ID + " AND [DeletedOn] = 'false' AND [Token] = " + secretToken + " ";
            command.Connection = connection;
            reader = command.ExecuteReader();

            DateTime ExpirationDate = DateTime.Now;

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ExpirationDate = Convert.ToDateTime(reader["Expiry"]);
                }
            }
            if (reader != null)
            {
                reader.Close();
            }

            //DateTime expiryDate = DateTime.ParseExact(expiry, "dd.MM.yy hh:mm:ss", CultureInfo.InvariantCulture);
            DateTime TimeDate = DateTime.Now;
            //Check DateTime

            if ((TimeDate - ExpirationDate).TotalMinutes < 5)
            {

                Session["SessionUsername"] = "User";
                int UserId = Convert.ToInt32(reader["UserId"]);

                //Get All Posts where Post.user_id == User.Id
                command.CommandText = "SELECT [Title] ,[Description] ,[Content] ,[CreateOn] ,[Title], [ModifiedOn] FROM dbo.Post WHERE [UserId]=" + UserId.ToString();
                reader = command.ExecuteReader();
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
                    ViewBag.Message = "Wrong Credentials";
                }

                //Create new Model, and fill in db informations
                Models.DashboardModel ViewDashboardModel = new Models.DashboardModel();
                ViewDashboardModel.Postlist = Posts;

                return RedirectToAction("Dashboard", "User");
            }
            else
            {
                ViewBag.Message = "Wrong Token";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}