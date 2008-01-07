<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DevicePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DevicePanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="uc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" CellSpacing="0" BorderWidth="0px">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Width="100%">
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
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom" Width="100%">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel5" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="AE Title"  CssClass="CSSTextBoxLabel" EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="AETitleFilter" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="IP Address"  CssClass="CSSTextBoxLabel" EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="IPAddressFilter" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by IP Address"></asp:TextBox></td>
                                            <td align="left" valign="bottom">
                                                <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" ToolTip="Show Enabled devices only" CssClass="CSSCheckBox" /></td>
                                            <td align="left" valign="bottom">
                                                <br />
                                                <asp:CheckBox ID="DHCPOnlyFilter" runat="server" Text="DHCP" ToolTip="Show only devices using DHCP" CssClass="CSSCheckBox"  /></td>
                                            <td align="right" valign="bottom">
                                                <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                                    OnClick="FilterButton_Click" ToolTip="Filter" /></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell ColumnSpan="2">
                        <uc1:DeviceGridView ID="DeviceGridViewControl1" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <uc8:GridPager ID="GridPager1" runat="server"></uc8:GridPager>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
