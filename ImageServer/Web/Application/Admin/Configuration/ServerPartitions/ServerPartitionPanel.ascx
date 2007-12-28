<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPanel" %>
<%@ Register Src="../../../Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc1" %>
<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel"
    TagPrefix="uc2" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" CssClass="PagePanel">
            <asp:Table ID="Table" runat="server" Width="100%">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Width="100%">
                        <asp:Panel ID="Panel2" runat="server" CssClass="PageToolbarPanel" Wrap="False">
                            <asp:ImageButton ID="AddButton" runat="server" ImageUrl="~/images/icons/AddEnabled.png"
                                AlternateText="Add" OnClick="AddButton_Click" />
                            <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/images/icons/EditEnabled.png"
                                OnClick="EditButton_Click" AlternateText="Edit" />
                            <asp:ImageButton ID="RefreshButton" runat="server" ImageUrl="~/images/icons/RefreshEnabled.png"
                                OnClick="RefreshButton_Click" AlternateText="Refresh" /></asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="PageFilterPanel">
                            <table cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label1" runat="server" Text="AE Title" ></asp:Label><br />
                                        <asp:TextBox ID="AETitleFilter" runat="server" Width="100px"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="Label2" runat="server" Text="Description" Width="71px"></asp:Label><br />
                                        <asp:TextBox ID="DescriptionFilter" runat="server" Width="100px"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="Label3" runat="server" Text="Folder" ></asp:Label><br />
                                        <asp:TextBox ID="FolderFilter" runat="server" Width="100px"></asp:TextBox>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" Width="92px" />
                                    </td>
                                    <td style="width: 50px" align="right" valign="bottom">
                                        <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                            OnClick="FilterButton_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell ColumnSpan="2">
                        <uc2:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <uc1:GridPager ID="GridPager" runat="server"></uc1:GridPager>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
