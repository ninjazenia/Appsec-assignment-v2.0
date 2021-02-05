<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="Appsec_assignment_v2._0.SignUp"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>sign up</title>
    <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;
            if (str.length > 0) {
                document.getElementById("lbl_pwdchecker").style.display = "block";
            }

            if (str.length < 8) {
                document.getElementById('lbl_pwdchecker').innerHTML = "Password length must be longer than 8 characters";
                document.getElementById('lbl_pwdchecker').style.color = "Red";
                return ("too short");
            }

            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "password require 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "password require one upper case";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_uppercase");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "password require one lower case";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_lowercase");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "password require one specialcase";
                document.getElementById("lbl_pwdchecker").style.color = "red";
                return ("no_special");
            } else {
                document.getElementById('lbl_pwdchecker').innerHTML = "Excellent";
                document.getElementById('lbl_pwdchecker').style.color = "Blue";
            }


        }
        function validatecreditcard() {
            var str = document.getElementById('<%=tb_creditcard.ClientID%>').value;
            if (str.search(/[0-9]{16}/) == -1){
                document.getElementById("creditcard1").innerHTML = "Invalid credit card";
                document.getElementById("creditcard1").style.color = "Red";
                return false;
            }
            
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="firstname" runat="server" >first name:</asp:Label>
                
            <asp:TextBox ID="tb_firstname" runat="server" width ="200px" ></asp:TextBox>
           <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Style="top: 15px;
        left: 345px; position: absolute; height: 26px; width: 162px" ErrorMessage="first name required"
        ControlToValidate="tb_firstname"></asp:RequiredFieldValidator>
            <br />
            <asp:Label ID="lastname" runat="server" >last name:</asp:Label>
            <asp:TextBox ID="tb_lastname" runat="server" width ="200px" ></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"  ErrorMessage="last name required"
        ControlToValidate="tb_lastname"></asp:RequiredFieldValidator>
            <br />
            <asp:Label ID="creditcard" runat="server" >credit card:</asp:Label>
            <asp:TextBox ID="tb_creditcard" runat="server" width ="200px" onkeyup="javascript:validatecreditcard()"></asp:TextBox>
            <asp:Label ID="creditcard1" runat="server" width ="200px" ></asp:Label>
           


            <br />
            <asp:Label ID="email" runat="server" >email</asp:Label>
            <asp:TextBox ID="tb_email" runat="server" width ="200px" TextMode="Email"></asp:TextBox>
            <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="tb_email" ErrorMessage="Invalid Email Format"></asp:RegularExpressionValidator>
             <asp:Label ID="lbl_emailchecker" runat="server" >yes</asp:Label>
            <br />
            <asp:Label ID="password" runat="server" >password</asp:Label>
            <asp:TextBox ID="tb_password" runat="server" width ="200px" onkeyup="javascript:validate()" TextMode="Password"></asp:TextBox>
            <p id="lbl_pwdchecker" style = "display:none;"></p>
            
            <br />
             <asp:Label ID="dob" runat="server" >date of birth</asp:Label>
            <asp:TextBox ID="tb_dob" runat="server" width ="200px" ></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"  ErrorMessage="dob required"
        ControlToValidate="tb_dob"></asp:RequiredFieldValidator>
                   
           
            <br />   
            <asp:Label ID="tb_pwdchecker" runat="server" Text="Label">message</asp:Label>
            <br />
            <asp:Label ID="lbl_gscore" runat="server" Text="" style ="visibility:hidden"></asp:Label>
            <br />
            <asp:Button ID="checkpass" runat="server" Text="register" OnClick="checkpass_Click" />
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            

            
        </div>
    </form>
    <script>
        grecaptcha.ready(function () {
            grecaptcha.execute('6Lel5C0aAAAAAMt82dZ8Zppfz0NuDfoKDUTYXEis', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</body>
        
</html>
