<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGroupsPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel" %>

<%@ Register Src="UserGroupsGridPanel.ascx" TagName="UserGroupsGridPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
         
            <asp:Table ID="Table1" runat="server">
                <asp:TableRow>
                    <asp:TableCell Wrap="false">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="right" valign="bottom">
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="Label3" runat="server" Text="Group Name" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="GroupName" runat="server" CssClass="SearchTextBox"></asp:TextBox>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" /></asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td>
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddUserGroupButton" runat="server" SkinID="AddButton" OnClick="AddUserGroupButton_Click"/>
                                        <ccUI:ToolbarButton ID="EditUserGroupButton" runat="server" SkinID="EditButton" OnClick="EditUserGroupButton_Click"/>
                                        <ccUI:ToolbarButton ID="DeleteUserGroupButton" runat="server" SkinID="DeleteButton" OnClick="DeleteUserGroupButton_Click"/>
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>


                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:UserGroupsGridPanel ID="UserGroupsGridPanel" runat="server" height="500px" /></td></tr>
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
