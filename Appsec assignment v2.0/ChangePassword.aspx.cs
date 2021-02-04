using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Appsec_assignment_v2._0
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDBConnectionString =System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["StatusMessage"] != null){
                lblmsg.Text = Session["StatusMessage"].ToString();
            }


        }
        protected void btnchangepass_Click(object sender, EventArgs e)
        {   
            //decrypt
            string pwd = txtcurrentpass.Text.ToString().Trim();
            string userid = tb_email.Text.ToString().Trim();
            var newdate = DateTime.Now;
            string olddate = forceChangePassword(userid);
            var comparedate = DateTime.Parse(olddate);
            var yes = ((comparedate - newdate).TotalMinutes < 15);
            if ((newdate-comparedate).TotalMinutes < 5)
            {
                lblmsg.Text = "You just recently changed your password";
                return;

            }
            else {
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);
                //encrypt

                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string encryptpwdWithSalt = pwd + dbSalt;
                    byte[] encrypthashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(encryptpwdWithSalt));
                    string userHash = Convert.ToBase64String(encrypthashWithSalt);
                    if (userHash.Equals(dbHash))
                    {

                        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                        byte[] saltByte = new byte[8];
                        //Fills array of bytes with a cryptographically strong sequence of random values.
                        rng.GetBytes(saltByte);
                        salt = Convert.ToBase64String(saltByte);
                        SHA512Managed hashing1 = new SHA512Managed();
                        string pwdWithSalt = pwd + salt;
                        byte[] plainHash = hashing1.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                        byte[] hashWithSalt = hashing1.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        finalHash = Convert.ToBase64String(hashWithSalt);
                        RijndaelManaged cipher = new RijndaelManaged();
                        cipher.GenerateKey();
                        Key = cipher.Key;
                        IV = cipher.IV;


                        Changepassword();


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
        protected void Changepassword()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Account SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt ,DateTimeRegistered = @Datetime WHERE Email = @Email"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@Datetime", DateTime.Now);

                            cmd.Connection = connection;
                            connection.Open();
                            cmd.ExecuteNonQuery();
                            lblmsg.Text = "Password changed successfully";
                            connection.Close();
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

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
    }
}