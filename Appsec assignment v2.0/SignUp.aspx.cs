using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using static Appsec_assignment_v2._0.Login;

namespace Appsec_assignment_v2._0
{
    public partial class SignUp : System.Web.UI.Page
    {
        string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        bool lockOut;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private int checkPassword(string password)
        {
            int score = 0;
            if (password.Length >= 8)
            {
                score++;

            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;

            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;

            }

            if (Regex.IsMatch(password, "[0-9]"))
            {

                score++;

            }
            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }
            return score;




        }

        protected void checkpass_Click(object sender, EventArgs e)
        {
            string pwd = tb_password.Text.ToString().Trim(); ;
            int scores = checkPassword(tb_password.Text);

            string status = "";
            switch (scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Excellent";
                    break;
                default:
                    break;
            }
            tb_pwdchecker.Text = "Status : " + status;
            if (scores < 4)
            {
                tb_pwdchecker.ForeColor = Color.Red;
                return;
            }

            else
            {
                if (ValidateCaptcha() )
                {
                    tb_pwdchecker.ForeColor = Color.Green;
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    lockOut = false;

                    createAccount();
                }

            }


        }
        public void createAccount()
        {



            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@Email,@FirstName,@LastName,@CreditCard,@PasswordHash,@PasswordSalt,@EmailVerified,@DateTimeRegistered,@DOB,@IV,@Key,@accountLockOut)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", tb_firstname.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", tb_lastname.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCard", Convert.ToBase64String(encryptData(tb_creditcard.Text.Trim())));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DOB", tb_dob.Text.Trim());
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Parameters.AddWithValue("@accountLockOut", lockOut);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            Response.Redirect("Login.aspx", false);
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
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
    //private Boolean uniqueEmail()
    //{
    //    Boolean valid = true;
    //    DataSet dset = new DataSet();
    //    DataSet profileDset = new DataSet();
    //    SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ToString());
    //    using (conn)
    //    {
    //        conn.Open();
    //        SqlDataAdapter adapter = new SqlDataAdapter();

    //        SqlCommand cmd = new SqlCommand("SELECT Id, Email, FirstName, LastName, CreditCard, PasswordHash, PasswordSalt, DOB FROM Account WHERE lower(Email) = @Email", conn);
    //        cmd.Parameters.AddWithValue("@Email", tb_email.Text.ToLower());
    //        cmd.CommandType = CommandType.Text;
    //        adapter.SelectCommand = cmd;
    //        adapter.Fill(dset);

    //    }

    //    if (dset.Tables[0].Rows.Count > 0)
    //    {

    //        lbl_emailchecker.Text = "Email is already a registered account!";
    //        valid = false;
               
    //    }
    //    else { lbl_emailchecker.Text = ""; }
    //    return valid;
    //}
    }
}
