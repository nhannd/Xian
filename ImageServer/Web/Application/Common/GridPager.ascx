<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.GridPager" %>
<asp:Panel ID="Panel1" runat="server" CssClass="CSSGridPagerPanelContainer">
<asp:Panel ID="Panel2" runat="server" CssClass="CSSGridPagerPanelBorder">
<asp:Panel ID="Panel3" runat="server" CssClass="CSSGridPagerPanelContent">
        
        <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td align="center" valign="middle">
                        <asp:Label ID="ItemCountLabel" runat="server" Text="Label"></asp:Label></td>
                    <td align="center" valign="middle">
                        <asp:Label ID="PageCountLabel" runat="server" Text="Label"></asp:Label></td>
                    <td align="right"  valign="bottom">
                        <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page" OnCommand="PageButtonClick"/>
                        <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page" OnCommand="PageButtonClick"/></td>
                </tr>
                
            </table>
        </asp:Panel>

</asp:Panel>

</asp:Panel>

            
