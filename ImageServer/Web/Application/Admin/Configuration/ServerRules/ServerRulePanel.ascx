<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRulePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRulePanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="gridPager" %>
<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="grid" %>
<asp:UpdatePanel ID="ServerRuleUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                BorderWidth="0px">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell ColumnSpan="1" Width="100%" HorizontalAlign="Left" VerticalAlign="Bottom">
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarContent">
                                    <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/icons/AddEnabled.png"
                                        AlternateText="Add" OnClick="AddButton_Click" />
                                    <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/icons/EditEnabled.png"
                                        OnClick="EditButton_Click" AlternateText="Edit" />
                                    <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="~/images/icons/DeleteEnabled.png"
                                        OnClick="DeleteButton_Click" AlternateText="Delete" />
                                    <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/icons/RefreshEnabled.png"
                                        OnClick="RefreshButton_Click" AlternateText="Refresh" />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel5" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Type" CssClass="CSSTextBoxLabel" EnableViewState="False" /><br />
                                                <asp:DropDownList ID="RuleTypeDropDownList" runat="server" CssClass="CSSFilterDropDownList" ToolTip="Filter by type" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Apply Time" CssClass="CSSTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" CssClass="CSSFilterDropDownList" ToolTip="Filter by apply time"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <br />
                                                <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" ToolTip="Show Enabled Rules only"
                                                    CssClass="CSSCheckBox" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <br />
                                                <asp:CheckBox ID="DefaultOnlyFilter" runat="server" Text="Default" ToolTip="Show only default rules"
                                                    CssClass="CSSCheckBox" />
                                            </td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="FilterButtonContainer">
                                                    <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                                        OnClick="FilterButton_Click" ToolTip="Filter" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2" VerticalAlign="Top">
                        <grid:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <gridPager:GridPager ID="GridPager" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
