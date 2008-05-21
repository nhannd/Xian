<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServiceLockPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServiceLocks.ServiceLockPanel" %>
<%@ Register Src="ServiceLockGridView.ascx" TagName="ServiceLockGridView" TagPrefix="localAsp" %>
<%@ Register Src="EditServiceLockDialog.ascx" TagName="EditServiceLockDialog" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="2" style="border-color: #6699CC"
                BorderWidth="2px">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Wrap="false" Width="100%">
                        <asp:UpdatePanel ID="ToolbarUpdatePanel" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server" CssClass="ToolbarPanelContainer">
                                <asp:Panel ID="Panel4" runat="server" CssClass="ToolbarContent">
                                
                                 <ccUI:ToolbarButton
                                        ID="EditToolbarButton" runat="server" 
                                        EnabledImageURL="~/images/icons/EditEnabled.png" 
                                        DisabledImageURL="~/images/icons/EditDisabled.png"
                                        OnClick="EditButton_Click" AlternateText="Edit a service"
                                        />
                                </asp:Panel>
                            </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom" Wrap="false">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="FilterPanelContainer">
                            <asp:Panel ID="Panel5" runat="server" CssClass="FilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="FilterPanelContent" DefaultButton="FilterToolbarButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="Type" CssClass="FilterTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:DropDownList ID="TypeDropDownList" runat="server" CssClass="FilterDropDownList">
                                                </asp:DropDownList>
                                                </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Status" CssClass="FilterTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="FilterDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="FilterButtonContainer">                                                        
                                                    <ccUI:ToolbarButton
                                                        ID="FilterToolbarButton" runat="server" 
                                                        EnabledImageURL="~/images/icons/QueryEnabled.png" 
                                                        DisabledImageURL="~/images/icons/QueryDisabled.png"
                                                        OnClick="FilterButton_Click" Tooltip="Filter services"
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
                        <asp:Panel ID="Panel7" runat="server" CssClass="GridViewPanelContainer" >
                                    <localAsp:ServiceLockGridView  ID="ServiceLockGridViewControl" Height="500px" runat="server" />
                        </asp:Panel>
                        
                        
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <ccAsp:GridPager ID="GridPager" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        
        <ccAsp:ConfirmationDialog ID="ConfirmEditDialog" runat="server" />
        <localAsp:EditServiceLockDialog ID="EditServiceLockDialog" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
