<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Test.TestPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="Message"></asp:Label>
        <asp:Button runat="server" Text="Logout" OnClick="Logout" />
        <hr />
        Old Password <asp:TextBox runat="server" ID="OldPassword"></asp:TextBox>
        New Password <asp:TextBox runat="server" ID="NewPassword"></asp:TextBox>
        <asp:Button runat="server" ID="ChangePassword" Text="Change Password" OnClick="ChangePasswordClicked" />
        <asp:Label runat="server" ID="ChangePasswordMessage"></asp:Label>
        <hr />
        Username <asp:TextBox runat="server" ID="DeleteUserName"></asp:TextBox>
        <asp:Button runat="server" ID="Button1" Text="Delete User" OnClick="DeleteUserClicked" />
        <asp:Label runat="server" ID="DeleteUserMessage"></asp:Label>
        <hr />
        
        Username <asp:TextBox runat="server" ID="ResetPasswordUserName"></asp:TextBox>
        <asp:Button runat="server" ID="Button2" Text="Reset Password" OnClick="ResetPasswordClicked" />
        <asp:Label runat="server" ID="ResetPasswordMessage"></asp:Label>
        
        <hr />
        Login<asp:TextBox runat="server" ID="NewUserLoginId"></asp:TextBox>
        Name<asp:TextBox runat="server" ID="NewUserName"></asp:TextBox>
        Groups 
        <asp:ListBox runat="server" ID="NewUserGroupListBox" SelectionMode="Multiple" />    
        <asp:Button runat="server" ID="NewUserButton" Text="Create User" OnClick="NewUserButtonClicked" />
        <asp:Button runat="server" ID="UpdateUserButton" Text="Update User" OnClick="UpdateUserButtonClicked" />
        <asp:Label runat="server" ID="NewUserMessage"></asp:Label>
        
        
    </div>
    </form>
</body>
</html>
