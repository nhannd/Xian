<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc1" %>
<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel"
    TagPrefix="uc2" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="100%">
            <asp:Table ID="Table" runat="server" Width="100%" Height="100%" CellPadding="0" CellSpacing="0"
                BorderWidth="0px">
                <asp:TableHeaderRow VerticalAlign="top">
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="bottom" Width="100%">
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel5" runat="server" CssClass="CSSToolbarContent">
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
                    <asp:TableHeaderCell HorizontalAlign="right" Width="100%" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="AETitleFilter" runat="server" CssClass="CSSFilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="Description" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="CSSFilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label3" runat="server" Text="Folder" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="FolderFilter" runat="server" CssClass="CSSFilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" CssClass="CSSCheckBox" />
                                            </td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="FilterButtonContainer" runat="server"  CssClass="FilterButtonContainer">
                                                    <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                                        OnClick="FilterButton_Click" />
                                                 </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2">
                        <uc2:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow VerticalAlign="bottom">
                    <asp:TableCell ColumnSpan="2">
                        <uc1:GridPager ID="GridPager" runat="server"></uc1:GridPager>
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
