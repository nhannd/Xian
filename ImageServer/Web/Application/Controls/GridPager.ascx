<%@ Control Language="C#" AutoEventWireup="true" Codebehind="GridPager.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.GridPager" %>
<table width="100%" cellpadding="0" cellspacing="0" class="GlobalGridPager">
    <tr>
        <td align="left" style="padding-left: 6px;">
                                            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.top)
                                   { %>
            <table cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="bottom">
                        <asp:Image runat="server" ImageUrl="~/App_Themes/Default/images/Controls/GridView/GridViewPagerTotalStudiesLeft.png" />
                    </td>
                    <td>

                        <div style="background: #e1eff7; border-bottom: solid 1px #b8d9ee; padding-top: 1px;
                            padding-bottom: 1px; width: 175px; text-align: center;">
                            <asp:Label ID="ItemCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
                        </div>

                    </td>
                    <td valign="bottom">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/Default/images/Controls/GridView/GridViewPagerTotalStudiesRight.png" />
                    </td>
                </tr>
            </table>
                        <%} %>            
        </td>
        <td align="center">
            <% if (PagerPosition == ImageServerConstants.GridViewPagerPosition.top)
               { %>
            <asp:UpdateProgress ID="SearchUpdateProgress" runat="server" AssociatedUpdatePanelID="SearchUpdatePanel"
                DisplayAfter="50">
                <ProgressTemplate>
                    <asp:Image ID="Image10" runat="server" SkinID="Searching" />
                </ProgressTemplate>
            </asp:UpdateProgress>
            <%} %>
        </td>
        <td align="right" style="padding-right: 6px; padding-bottom: 2px; padding-top: 0px;">
            <table cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="top" style="padding-top: 1px;">
                        <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>
                    <td>
                        <div style="background: #e1eff7; border-bottom: solid 1px #b8d9ee; padding-left: 6px;
                            padding-right: 6px; padding-top: 1px; margin-bottom: 1px;">
                            <asp:Label ID="Label3" runat="server" Text="Page" CssClass="GlobalGridPagerLabel" />
                            <asp:TextBox ID="CurrentPage" runat="server" Width="35px" CssClass="GridViewTextBox"
                                Style="font-size: 12px; text-align: right;" />
                            <asp:Label ID="PageCountLabel" runat="server" Text="Label" CssClass="GlobalGridPagerLabel" />
                            <aspAjax:MaskedEditExtender runat="server" ID="CurrentPageMask" MaskType="Number"
                                TargetControlID="CurrentPage" Mask="999999" PromptCharacter="" AutoComplete="false"
                                MessageValidatorTip="false" AcceptNegative="None" InputDirection="LeftToRight" />
                        </div>
                    </td>
                    <td valign="top" style="padding-right: 1px; padding-top: 1px;">
                        <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                            OnCommand="PageButtonClick" CssClass="GlobalGridPagerLink" />
                    </td>
                    <td>
                        <%-- This Link Button is used to submit the Page from the TextBox when the user clicks enter on the text box. --%>
                        <asp:LinkButton ID="ChangePageButton" runat="server" CommandArgument="ChangePage"
                            CommandName="Page" OnCommand="PageButtonClick" Text="" />
                    </td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
</table>
