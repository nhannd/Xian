<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Codebehind="Theme.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Theme"
    Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Theme
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <center>
        <br />
        <br />
        <br />
        <br />
        <br />
        Select Your Theme:
        <asp:DropDownList ID="Themes" runat="server" OnSelectedIndexChanged="ThemeSelectedIndexChanged"
            AutoPostBack="true">
            <asp:ListItem Text="Standard" Value="ClearCanvas" />
            <asp:ListItem Text="Night Safari" Value="NightSafari" />
        </asp:DropDownList>
    </center>
</asp:Content>
