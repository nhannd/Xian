<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsPanel" %>
<%@ Register Src="FileSystemsGridView.ascx" TagName="FileSystemsGridView" TagPrefix="uc1" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Table ID="Table" runat="server" Width="100%" CellSpacing="0" CellPadding="0">
            <asp:TableHeaderRow>
                <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Width="100%">
                    <asp:Panel ID="Panel1" runat="server" CssClass="CSSToolbarPanelContainer">
                        <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                            <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarContent">
                                <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/icons/AddEnabled.png"
                                    AlternateText="Add" OnClick="AddButton_Click" />
                                <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/icons/EditEnabled.png"
                                    OnClick="EditButton_Click" AlternateText="Edit" />
                                <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/icons/RefreshEnabled.png"
                                    OnClick="RefreshButton_Click" AlternateText="Refresh" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                </asp:TableHeaderCell>
                <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom">
                    <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                        <asp:Panel ID="Panel5" runat="server" CssClass="CSSFilterPanelBorder">
                            <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextBoxLabel"
                                                EnableViewState="False"></asp:Label><br />
                                            <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
                                        <td align="left">
                                            <asp:Label ID="Label2" runat="server" Text="Tiers" CssClass="CSSTextBoxLabel" EnableViewState="False"></asp:Label><br />
                                            <asp:DropDownList ID="TiersDropDownList" runat="server" CssClass="CSSFilterTextBox">
                                            </asp:DropDownList></td>
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
                    <uc1:FileSystemsGridView ID="FileSystemsGridView1" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="2">
                    <uc8:GridPager ID="GridPager1" runat="server" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ContentTemplate>
</asp:UpdatePanel>
