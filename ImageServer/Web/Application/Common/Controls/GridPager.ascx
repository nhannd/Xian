<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.GridPager" %>

<table width="100%" cellpadding="0" cellspacing="0" class="GlobalGridPager">
    <tr>
        <td>
            <asp:Label ID="ItemCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
            <asp:Label ID="PageCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
        </td>
        <td align="right" style="padding-right: 8px;">
            <asp:LinkButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
            <asp:Label ID="LineSpacerLabel" runat="server" CssClass="GlobalGridPagerLink" Text=" | " />                
            <asp:LinkButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
        </td>
    </tr>
</table>
