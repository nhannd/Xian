<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="ChangePasswordDialog.ascx" TagName="ChangePasswordDialog" TagPrefix="localAsp" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ClearCanvas ImageServer - Login</title>
</head>
<body class="LoginBody">

    <form runat="server">

    <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true">
    </asp:ScriptManager>    

    <div id="LoginSplash">
        <div id="LoginCredentials">
        <table>
            <tr>
                <td colspan="2" align="right"><asp:Panel style="background: #8FC3E4; padding: 2px 7px 2px 7px;" runat="server" ID="LoginErrorPanel" Visible="false"><asp:Label runat="server" ID="LoginError" ForeColor="red" Text="Invalid username/password combination." /></asp:Panel></td>
            </tr>        
            <tr>
            <td>Username:</td>
            <td><asp:TextBox runat="server" ID="UserName" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr>
            <tr>
            <td>Password:</td>
            <td><asp:TextBox runat="server" ID="Password" TextMode="Password" Width="100" CssClass="LoginTextInput"></asp:TextBox></td>
            </tr> 
            <tr>
                <td colspan="2" align="right"><asp:Button runat="server" ID="LoginButton" OnClick="LoginClicked"  Text="Login" CssClass="LoginButton"/></td>
            </tr>               
            <tr>
                <td colspan="2" align="right" ><asp:LinkButton runat="server" CssClass="LoginLink" OnClick="ChangePassword">Change Password</asp:LinkButton></td>            
            </tr>   
        </table>
        </div>
                        
            
    </div>
    
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <localAsp:ChangePasswordDialog runat="server" id="ChangePasswordDialog" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    </form>
</body>
</html>
