<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorityGroup.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Test.AuthorityGroup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label runat="server" ID="Login"  />
    <asp:Button runat="server" ID="Logout" Text="Logout" OnClick="LogoutClicked" />
    
    <hr />
    
    Name:<asp:TextBox runat="server" ID="NewTokenNameTextBox"></asp:TextBox>
    Description:<asp:TextBox runat="server" ID="NewTokenDescriptionTextBox"></asp:TextBox>
    <asp:Button runat="server" ID="NewToken" OnClick="NewTokenClicked" Text="Create Token"/>
    
    <hr />
    Group:<asp:TextBox runat="server" ID="NewGroupName"></asp:TextBox>
    Tokens:
    <asp:ListBox runat="server" ID="NewGroupTokenListBox" SelectionMode="Multiple" />
    <asp:Button runat="server" ID="NewGroupCreate" OnClick="CreateNewGroupClicked" Text="Add Group"/>
    
    <hr />
    
    <table>
    <tr>
    <td valign="top" style="font-size: x-large">
    Groups
    <asp:GridView runat="server" ID="List" style="font-size: medium" CellPadding="4" ForeColor="#333333" GridLines="None" >
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    </asp:GridView>
    </td>
    <td valign="top" style="font-size: x-large">
    Tokens:
    <asp:GridView runat="server" ID="TokenList" style="font-size: medium" CellPadding="4" ForeColor="#333333" GridLines="None" >
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    </asp:GridView>
    </td>
    </tr>
    </table>
    </div>
    </form>
</body>
</html>
