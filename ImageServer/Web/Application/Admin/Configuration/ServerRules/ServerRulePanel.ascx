<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRulePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRulePanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="gridPager" %>
<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="grid" %>
<asp:UpdatePanel ID="ServerRuleUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" CssClass="PagePanel">
            <asp:Table ID="Table" runat="server" Width="100%">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell ColumnSpan="1" Width="100%"  HorizontalAlign="Left" VerticalAlign="Bottom">
                        <asp:Panel ID="ToolbarPanel" runat="server" CssClass="PageToolbarPanel"
                            Wrap="False">
                            <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/icons/AddEnabled.png"
                                AlternateText="Add" OnClick="AddButton_Click" />
                            <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/icons/EditEnabled.png"
                                OnClick="EditButton_Click" AlternateText="Edit" />
                            <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/images/icons/DeleteEnabled.png"
                                OnClick="DeleteButton_Click" AlternateText="Delete" />
                            <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/icons/RefreshEnabled.png"
                                OnClick="RefreshButton_Click" AlternateText="Refresh" />
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="PageFilterPanel">
                            <table cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label1" runat="server" Text="Rule Type" Width="100px" EnableViewState="False" /><br />
                                        <asp:DropDownList ID="RuleTypeDropDownList" runat="server" Width="100px" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label2" runat="server" Text="Rule Apply Time" Width="100px" EnableViewState="False"></asp:Label><br />
                                        <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" Width="100px" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <br />
                                        <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" ToolTip="Show Enabled Rules only" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <br />
                                        <asp:CheckBox ID="DefaultOnlyFilter" runat="server" Text="Default" ToolTip="Show only default rules" />
                                    </td>
                                    <td style="width: 50px" align="right" valign="bottom">
                                        <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                            OnClick="FilterButton_Click" ToolTip="Filter" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2" VerticalAlign="Top">
                        <grid:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow>
                    <asp:TableCell ColumnSpan="2">
                        <gridPager:GridPager ID="GridPager" runat="server" />
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
