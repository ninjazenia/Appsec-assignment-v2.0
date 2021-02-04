<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Appsec_assignment_v2._0.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
                <fieldset>
                <legend>Homepage</legend>
                <br />
                <p>
 
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                    <br />
                <asp:Label ID="user" runat="server" EnableViewState="false">UserID:</asp:Label>
                <asp:Label ID="lbl_UserID" runat="server" EnableViewState="false"></asp:Label>
      
                <br />'
                <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="LogoutMe" Visible="false"/>
                </p>
                </fieldset>
        </div>
    </form>
</body>
</html>
