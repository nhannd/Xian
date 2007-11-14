<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.FileSystems.FileSystemsToolBar" Codebehind="FileSystemsToolBar.ascx.cs" %>


<asp:Panel ID="Panel1" runat="server" CssClass="ToolBar" style="display: block; overflow: visible;" Wrap="False">
    <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/add2.gif"   AlternateText="Add" OnClick="AddButton_Click" />
    <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/edit2.gif" OnClick="EditButton_Click" AlternateText="Edit" />
    <!--<asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/images/delete2.gif"
        OnClick="DeleteButton_Click" AlternateText="Delete" />-->
    <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/refresh2.gif"
        OnClick="RefreshButton_Click" AlternateText="Refresh" />

</asp:Panel>