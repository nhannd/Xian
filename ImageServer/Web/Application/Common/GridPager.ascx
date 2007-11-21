<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.GridPager" %>
<asp:Panel ID="Panel1" runat="server" Height="24px" Width="100%" BackColor="Gainsboro" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px">
    <table width="100%">
        <tr>
            <td align="center" valign="middle">
                <asp:Label ID="ItemCountLabel" runat="server" Text="Label"></asp:Label></td>
            <td align="center" valign="middle">
                <asp:Label ID="PageCountLabel" runat="server" Text="Label"></asp:Label></td>
            <td align="right" valign="middle">
                <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page" OnCommand="PageButtonClick" />
                <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page" OnCommand="PageButtonClick" /></td>
        </tr>
        
    </table>
</asp:Panel>
