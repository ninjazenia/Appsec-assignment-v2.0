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
            if (tb_creditcard.Text.Length != 16)
            {

            }
            else
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

    }
}
