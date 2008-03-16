<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsPanel" %>
<%@ Register Src="FileSystemsGridView.ascx" TagName="FileSystemsGridView" TagPrefix="uc1" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" TagPrefix="clearcanvas" %>
    
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel2" runat="server" Height="100%">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Width="100%">
                        <asp:Panel ID="Panel1" runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarContent">
                                    <clearcanvas:ToolbarButton
                                                        ID="AddToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/AddEnabled.png" 
                                                        DisabledImageURL="~/images/icons/AddDisabled.png"
                                                        OnClick="AddButton_Click" AlternateText="Add a filesystem"
                                                        />
                                     <clearcanvas:ToolbarButton
                                                        ID="EditToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/EditEnabled.png" 
                                                        DisabledImageURL="~/images/icons/EditDisabled.png"
                                                        OnClick="EditButton_Click" AlternateText="Edit a filesystem"
                                                        />
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
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter by description"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="Tiers" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" CssClass="CSSFilterDropDownList">
                                                </asp:DropDownList></td>
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
                <asp:TableRow Height="100%">
                    <asp:TableCell ColumnSpan="2">
                        <asp:Panel ID="Panel7" runat="server" CssClass="CSSGridViewPanelContainer" >
                                <asp:Panel ID="Panel8" runat="server" CssClass="CSSGridViewPanelBorder" >
                                    <uc1:FileSystemsGridView ID="FileSystemsGridView1" runat="server"  Height="500px"/>
                                </asp:Panel>                        
                        </asp:Panel>
                        
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <uc8:GridPager ID="GridPager1" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table> 
       </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
