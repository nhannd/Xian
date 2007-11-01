<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin_Configuration_DeviceToolBarControl" Codebehind="DeviceToolBarControl.ascx.cs" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel" Width="100%">
    <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/add2.gif" OnClick="AddButton_Click" AlternateText="Add" />
    <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/edit2.gif" OnClick="EditButton_Click" AlternateText="Edit" />
    <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/images/delete2.gif"
        OnClick="DeleteButton_Click" AlternateText="Delete" Enabled="False" />
    <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/refresh2.gif"
        OnClick="RefreshButton_Click" AlternateText="Refresh" /></asp:Panel>
