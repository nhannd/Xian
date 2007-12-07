<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchToolBar.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchToolBar" %>
<asp:Panel ID="Panel1" runat="server" CssClass="ToolBar" Style="display: block; overflow: visible;"
    Wrap="False">
    <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/icons/RefreshEnabled.png"
        OnClick="RefreshButton_Click" AlternateText="Refresh" />
</asp:Panel>
