<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AlertsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel" %>

<%@ Register Src="AlertsGridPanel.ascx" TagName="AlertsGridPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
         
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell Wrap="false">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="right" valign="bottom">
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label3" runat="server" Text="Component" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="ComponentFilter" runat="server" CssClass="SearchTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Insert Date" CssClass="SearchTextBoxLabel" />&nbsp;&nbsp;
                                                <asp:LinkButton ID="ClearInsertDateButton" runat="server" Text="X" CssClass="SmallLink" style="margin-left: 10px;"/><br />
                                                <ccUI:TextBox ID="InsertDateFilter" runat="server" ReadOnly="true" CssClass="SearchDateBox" />
                                            </td>                                            
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Category" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="CategoryFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Level" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="LevelFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            </td>                                                            
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click"/></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                        <ccUI:CalendarExtender ID="InsertDateCalendarExtender" runat="server" TargetControlID="InsertDateFilter"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>                                
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="DeleteAlertButton" runat="server" SkinID="DeleteButton" OnClick="DeleteAlertButton_Click" />
                                        <ccUI:ToolbarButton ID="DeleteAllAlertsButton" runat="server" SkinID="DeleteAllButton" OnClick="DeleteAllAlertsButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:AlertsGridPanel ID="AlertsGridPanel" runat="server" Height="500px" /></td></tr>
                                <tr><td style="border-top: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerBottom" runat="server" /></td></tr>                    
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>                
                </asp:TableRow>
            </asp:Table>
            
    </ContentTemplate>
</asp:UpdatePanel>
