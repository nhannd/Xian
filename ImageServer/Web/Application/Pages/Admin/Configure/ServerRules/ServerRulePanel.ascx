<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRulePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel" %>

<%@ Register Src="ServerRuleGridView.ascx" TagName="ServerRuleGridView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="ServerRuleUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="2" style="border-color: #6699CC"
                BorderWidth="2px">
                <asp:TableRow>
                    <asp:TableCell VerticalAlign="Bottom" Wrap="false">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Type" CssClass="SearchTextBoxLabel" EnableViewState="False" /><br />
                                                <asp:DropDownList ID="RuleTypeDropDownList" runat="server" CssClass="SearchDropDownList" ToolTip="Search by type" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Apply Time" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" CssClass="SearchDropDownList" ToolTip="Search by apply time"/>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Status" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Default" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="DefaultFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            </td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="Panel2" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click"/></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                            </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                
                                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolbarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddServerRuleButton" runat="server" SkinID="AddButton" OnClick="AddServerRuleButton_Click" />
                                        <ccUI:ToolbarButton ID="EditServerRuleButton" runat="server" SkinID="EditButton" OnClick="EditServerRuleButton_Click" />
                                        <ccUI:ToolbarButton ID="DeleteServerRuleButton" runat="server" SkinID="DeleteButton" OnClick="DeleteServerRuleButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel1" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:ServerRuleGridView ID="ServerRuleGridViewControl" runat="server"  Height="500px"/></td></tr>
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
