<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerPartitions.ServerPartitionPanel" %>
<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel"
    TagPrefix="uc2" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="100%" style="border: solid 1px #6699CC">
            <asp:Table ID="Table" runat="server" Width="100%" Height="100%" CellPadding="0"
                BorderWidth="0px">
                <asp:TableHeaderRow VerticalAlign="top" >
                    <asp:TableHeaderCell VerticalAlign="bottom" HorizontalAlign="Left" Width="100%">
                        <asp:Panel ID="Panel2" runat="server" CssClass="ToolbarPanelContainer">
                                    <ccUI:ToolbarButton
                                                        ID="AddToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/AddEnabled.png" 
                                                        DisabledImageURL="~/images/icons/AddDisabled.png"
                                                        OnClick="AddButton_Click" AlternateText="Add a server partition"
                                                        />
                                    
                                    <ccUI:ToolbarButton
                                                        ID="EditToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/EditEnabled.png" 
                                                        DisabledImageURL="~/images/icons/EditDisabled.png"
                                                        OnClick="EditButton_Click" AlternateText="Edit the server partition"
                                                        />
                                    <ccUI:ToolbarButton
                                                        ID="DeleteToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/DeleteEnabled.png" 
                                                        DisabledImageURL="~/images/icons/DeleteDisabled.png"
                                                        OnClick="DeleteButton_Click" AlternateText="Delete the server partition"
                                                        />
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" Width="100%" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="FilterPanelContainer">
                                <asp:Panel ID="Panel6" runat="server" CssClass="FilterPanelContent" DefaultButton="FilterToolbarButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="AETitleFilter" runat="server" CssClass="FilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="Description" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="FilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label3" runat="server" Text="Folder" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="FolderFilter" runat="server" CssClass="FilterTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Status" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="FilterDropDownList">
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
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell ColumnSpan="2">
                        <asp:Panel ID="Panel7" runat="server" CssClass="GridViewPanelContainer" >
                                <asp:Panel ID="Panel8" runat="server" CssClass="GridViewPanelBorder" >
                                    <uc2:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" Height="500px"/>
                                </asp:Panel>                        
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow VerticalAlign="bottom">
                    <asp:TableCell ColumnSpan="2">
                        <ccAsp:GridPager ID="GridPager" runat="server"></ccAsp:GridPager>
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
