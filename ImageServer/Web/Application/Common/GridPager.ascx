<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.GridPager" %>
<asp:Panel ID="Panel1" runat="server" CssClass="GridPagerPanelContainer">
        <asp:Panel ID="Panel3" runat="server" CssClass="GridPagerPanelContent">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" valign="middle">
                        <asp:Label ID="ItemCountLabel" runat="server" Text="Label"></asp:Label></td>
                    <td align="center" valign="middle">
                        <asp:Label ID="PageCountLabel" runat="server" Text="Label"></asp:Label></td>
                    <td align="right" valign="bottom">
                        <asp:Panel runat="server" ID="GridPagerNavigation" CssClass="GridPagerNavigation">
                        <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                            OnCommand="PageButtonClick" />
                        <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                            OnCommand="PageButtonClick" />
                        </asp:Panel>
                    </td>
                         
                </tr>
            </table>
    </asp:Panel>
</asp:Panel>
