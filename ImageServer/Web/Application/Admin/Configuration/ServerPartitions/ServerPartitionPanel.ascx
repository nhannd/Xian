<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc1" %>
<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel"
    TagPrefix="uc2" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" 
    Assembly="ClearCanvas.ImageServer.Web.Common" %>


<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="100%">
            <asp:Table ID="Table" runat="server" Width="100%" Height="100%" CellPadding="0"
                BorderWidth="0px">
                <asp:TableHeaderRow VerticalAlign="top">
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="bottom" Width="100%">
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel5" runat="server" CssClass="CSSToolbarContent">
                                    <clearcanvas:ToolbarButton
                                                        ID="AddToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/AddEnabled.png" 
                                                        DisabledImageURL="~/images/icons/AddDisabled.png"
                                                        OnClick="AddButton_Click" AlternateText="Add a server partition"
                                                        />
                                    
                                    <clearcanvas:ToolbarButton
                                                        ID="EditToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/EditEnabled.png" 
                                                        DisabledImageURL="~/images/icons/EditDisabled.png"
                                                        OnClick="EditButton_Click" AlternateText="Edit a server partition"
                                                        />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" Width="100%" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent" DefaultButton="FilterToolbarButton">
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
                                                <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="FilterButtonContainer">
                                                    <clearcanvas:ToolbarButton
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
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2">
                        <asp:Panel ID="Panel7" runat="server" CssClass="CSSGridViewPanelContainer" >
                                <asp:Panel ID="Panel8" runat="server" CssClass="CSSGridViewPanelBorder" >
                                    <uc2:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" Height="500px"/>
                                </asp:Panel>                        
                        </asp:Panel>
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
