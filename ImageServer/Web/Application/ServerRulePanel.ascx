<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRulePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerRules.ServerRulePanel" %>
<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="grid" %>

<asp:UpdatePanel ID="ServerRuleUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0"
                BorderWidth="0px">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell ColumnSpan="1" Width="100%" HorizontalAlign="Left" VerticalAlign="Bottom">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ToolbarPanelContainer">
                                    <ccUI:ToolbarButton
                                                        ID="AddToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/AddEnabled.png" 
                                                        DisabledImageURL="~/images/icons/AddDisabled.png"
                                                        OnClick="AddButton_Click" AlternateText="Add a server rule"
                                                        />
                                    <ccUI:ToolbarButton
                                                        ID="EditToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/EditEnabled.png" 
                                                        DisabledImageURL="~/images/icons/EditDisabled.png"
                                                        OnClick="EditButton_Click" Tooltip="Edit a server rule"
                                                        />                    
                                    <ccUI:ToolbarButton
                                                        ID="DeleteToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/DeleteEnabled.png" 
                                                        DisabledImageURL="~/images/icons/DeleteDisabled.png"
                                                        OnClick="DeleteButton_Click" AlternateText="Delete a server rule"
                                                        />      
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="FilterPanelContainer">
                                <asp:Panel ID="Panel6" runat="server" CssClass="FilterPanelContent" DefaultButton="FilterToolbarButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Type" CssClass="FilterTextBoxLabel" EnableViewState="False" /><br />
                                                <asp:DropDownList ID="RuleTypeDropDownList" runat="server" CssClass="FilterDropDownList" ToolTip="Filter by type" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Apply Time" CssClass="FilterTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" CssClass="FilterDropDownList" ToolTip="Filter by apply time"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Status" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="FilterDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Default" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="DefaultFilter" runat="server" CssClass="FilterDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="FilterButtonContainer">
                                                    <ccUI:ToolbarButton
                                                        ID="FilterToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/QueryEnabled.png" 
                                                        DisabledImageURL="~/images/icons/QueryDisabled.png"
                                                        OnClick="FilterButton_Click" Tooltip="Filter/Refresh"
                                                        />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2" VerticalAlign="Top">
                        <asp:Panel ID="Panel7" runat="server" CssClass="GridViewPanelContainer" >
                                <asp:Panel ID="Panel8" runat="server" CssClass="GridViewPanelBorder" >
                                    <grid:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server"  Height="500px"/>
                                </asp:Panel>                        
                        </asp:Panel> 
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <ccAsp:GridPager ID="GridPager" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
