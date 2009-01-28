<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Test.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label runat="server" ID="CurrentUser" /><p></p>
        Username: <asp:TextBox runat="server" ID="UserName"></asp:TextBox>
        <br />d
        Password: <asp:TextBox runat="server" ID="Password" TextMode="Password"></asp:TextBox>
        <br />
        <asp:Button runat="server" ID="LoginButton" OnClick="LoginClicked"  Text="Login"/>
        <br />
        <asp:Label runat="server" ID="Result"></asp:Label>
        
    </div>
    </form>
</body>
</html>
