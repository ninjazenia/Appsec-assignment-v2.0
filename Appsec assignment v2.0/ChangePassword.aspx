<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Appsec_assignment_v2._0.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
       <div>
        Enter Email :
        <asp:TextBox ID="tb_email" runat="server" Width="120px" TextMode="Email"></asp:TextBox>
           <br />

        Enter Current Password :
        <asp:TextBox ID="txtcurrentpass" runat="server" Width="120px"></asp:TextBox>
         <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="txtcurrentpass" ErrorMessage="!!" ForeColor="Red" 
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
        </br>
        Enter New Password :
        <asp:TextBox ID="txtnewpass" runat="server" Width="120px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="txtnewpass" ErrorMessage="!!" ForeColor="Red" 
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
      
        </br>
         Confirm Password :
        <asp:TextBox ID="txtconfirmpass" runat="server" Width="120px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ControlToValidate="txtconfirmpass" ErrorMessage="!!" ForeColor="Red" 
                        SetFocusOnError="True"></asp:RequiredFieldValidator>
        <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ControlToCompare="txtnewpass" ControlToValidate="txtconfirmpass" 
                        ErrorMessage="password not same !!" ForeColor="Red"></asp:CompareValidator>

        </div>
          <asp:Button ID="btnchangepass" runat="server" Text="Change Password" 
                        onclick="btnchangepass_Click" CausesValidation="true"/>
        <asp:Button ID="Login" runat="server" Text="Login" OnClick="Login_Click" CausesValidation="false"/>
        <asp:Label ID="lblmsg" runat="server"></asp:Label>
    </form>
</body>
</html>
