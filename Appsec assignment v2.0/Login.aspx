<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Appsec_assignment_v2._0.Login"   %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login Form</title>
    <script src="https://www.google.com/recaptcha/api.js?render=6Lel5C0aAAAAAMt82dZ8Zppfz0NuDfoKDUTYXEis"></script>


</head>
<body>
    <form id="form1" runat="server">
        <div>
            <fieldset>
            <legend>Login</legend>
            <p>Email:<asp:TextBox ID="tb_email" runat="server"></asp:TextBox></p>
            <p>Password:<asp:TextBox ID="tb_pwd" runat="server" TextMode="Password"></asp:TextBox></p>
            <br />
            <br />
           <!--<div class="g-recaptcha" data-sitekey=" 6LctMwoaAAAAAI-ey8HwZMesNv5OqAkoOnH50dqv "></div>-->
            <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>
            <asp:Label ID="lblMessage" runat="server" EnableViewState ="false" Text="no"></asp:Label>
              <asp:Label ID="lbl_gscore" runat="server" Text="" style ="visibility:hidden"></asp:Label>
            <p><asp:Button ID="Button1" runat="server" Text="Login" Onclick="LoginMe"  />
                <asp:Button ID="Button2" runat="server" Text="Sign Up" OnClick="SignUpMe" />
                <asp:Button ID="Button3" runat="server" Text="ChangePassword" OnClick="Forget"/>
            </p>
                
            </fieldset>

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

