using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;
using System.Drawing;


namespace Appsec_assignment_v2._0
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }

        }
        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Lel5C0aAAAAAL2ZO3Q1qlX4Pyoo7F24Pl2AXSB2 &response=" + captchaResponse);
            Console.WriteLine("This is C#");
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        lbl_gscore.Text = jsonResponse.ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);

                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }



        }
        protected void LoginMe(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string pwd = tb_pwd.Text.ToString().Trim();
                string userid = tb_email.Text.ToString().Trim();
                if (userid == "" || pwd == "")
                {
                    lblMessage.Text = "Please enter your credentials.";
                    lblMessage.ForeColor = Color.Red;
                }
                else
                {
                    if (checkValidEmail(userid) == null)
                    {
                        lblMessage.Text = "You entered Email wrongly.Please try again";
                        lblMessage.ForeColor = Color.Red;
                    }
                    else
                    {
                        if (checkAccountLockout(userid) == "True")
                        {
                            lblMessage.Text = "Account is locked out.";
                            lblMessage.ForeColor = Color.Red;
                        }
                        else
                        {
                            var newdate = DateTime.Now;
                            string olddate = forceChangePassword(userid);
                            var comparedate = DateTime.Parse(olddate);
                            var yes = ((comparedate - newdate).TotalMinutes < 15);
                            Console.WriteLine(olddate);
                            if ((newdate - comparedate).TotalMinutes > 15)
                            {
                                var statusmsg = "Change your password";
                                Session["StatusMessage"] = statusmsg;
                                Response.Redirect("ChangePassword.aspx", false);
                                
                            }

                        //    if (){ }
                            
                            else {
                                SHA512Managed hashing = new SHA512Managed();
                                string dbHash = getDBHash(userid);
                                string dbSalt = getDBSalt(userid);
                                try
                                {
                                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                                    {
                                        string pwdWithSalt = pwd + dbSalt;
                                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                        string userHash = Convert.ToBase64String(hashWithSalt);
                                        if (userHash.Equals(dbHash))
                                        {
                                            Session["UserID"] = userid;
                                            Session["Time"] = yes;
                                            Session["LoggedIn"] = tb_email.Text.Trim();
                                            string guid = Guid.NewGuid().ToString();
                                            Session["AuthToken"] = guid;
                                            Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                            Response.Redirect("Home.aspx?Email=" + HttpUtility.UrlEncode(userid), false);
                                        }


                                        else
                                        {

                                            if (Session["LogInAttempt" + userid] == null)
                                            {
                                                Session["LogInAttempt" + userid] = 2;
                                                int intAttempt = (int)Session["LogInAttempt" + userid];
                                                lblMessage.Text = "Email or password is not valid. Please try again. You have " + intAttempt + " left.";
                                                lblMessage.ForeColor = Color.Red;


                                            }
                                            else
                                            {
                                                int intAttempt = (int)Session["LogInAttempt" + userid];
                                                intAttempt -= 1;
                                                Session["LogInAttempt" + userid] = intAttempt;
                                                lblMessage.Text = "Email or password is not valid. Please try again. You have " + intAttempt + " left.";
                                                lblMessage.ForeColor = Color.Red;
                                                if (intAttempt < 0)
                                                {
                                                    SqlConnection connection = new SqlConnection(MYDBConnectionString);
                                                    string sql = "UPDATE Account SET accountLockout = 1 WHERE Email=@Email";
                                                    SqlCommand command = new SqlCommand(sql, connection);
                                                    command.Parameters.AddWithValue("@Email", userid);
                                                    try
                                                    {
                                                        connection.Open();
                                                        SqlDataReader reader = command.ExecuteReader();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        throw new Exception(ex.ToString());
                                                    }
                                                    finally { connection.Close(); }
                                                    lblMessage.Text = "This account has been locked.";
                                                    lblMessage.ForeColor = Color.Red;
                                                }
                                                else
                                                {
                                                    lblMessage.Text = "Email or password is not valid. Please try again. You have " + intAttempt + " left.";
                                                    lblMessage.ForeColor = Color.Red;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally { }

                            }
                        }
                    }
                }

            }
        }
        
        protected string getDBHash(string userid)
        {
            
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }
        protected string getDBSalt(string userid)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }
        protected string checkValidEmail(string Email)
        {
            string e = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Email FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", Email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            if (reader["Email"] != DBNull.Value)
                            {
                                e = reader["Email"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return e;
        }
        protected string checkAccountLockout(string Email)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string c = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select accountLockout FROM Account WHERE Email=@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", Email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["accountLockout"] != null)
                        {
                            if (reader["accountLockout"] != DBNull.Value)
                            {
                                c = reader["accountLockout"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return c;
        }
        protected string forceChangePassword(string userid)
        {
            string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            string c = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select DateTimeRegistered  FROM Account WHERE Email =@Email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["DateTimeRegistered"] != null)
                        {
                            if (reader["DateTimeRegistered"] != DBNull.Value)
                            {
                                c = reader["DateTimeRegistered"].ToString();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return c;
        }


            protected void SignUpMe(object sender, EventArgs e)
        {
            Response.Redirect("SignUp.aspx", false);
        }

        protected void Forget(object sender, EventArgs e)
        {
            Response.Redirect("ChangePassword.aspx", false);
        }
    }
}