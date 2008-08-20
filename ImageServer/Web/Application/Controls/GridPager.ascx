<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.GridPager" %>

<table width="100%" cellpadding="0" cellspacing="0" class="GlobalGridPager">
    <tr>
        <td width="33%">
            <asp:Panel runat="server" style="padding-left: 8px;">
                <asp:Label ID="ItemCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
            </asp:Panel>
       </td>
        <td align="right" style="padding-right: 8px;" width="34%">
            <asp:LinkButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
            <asp:Label ID="Label2" runat="server" CssClass="GlobalGridPagerLink" Text=" | " />
                            <asp:Label ID="PageCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
            <asp:Label ID="Label1" runat="server" CssClass="GlobalGridPagerLink" Text=" | " />                                            
            <asp:LinkButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
        </td>
    </tr>
</table>
