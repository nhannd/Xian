<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.FileSystemsPanel" %>

<%@ Register Src="FileSystemsGridView.ascx" TagName="FileSystemsGridView" TagPrefix="localAsp" %>
    
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>   
            <asp:Table runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContentWithoutTabs" DefaultButton="SearchButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="Description" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search by description"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="Tiers" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click"/></asp:Panel>
                                            </td>
                                        </tr> 
                                    </table>
                            </asp:Panel>
                    </asp:TableCell> 
                 </asp:TableRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell>
                        <table width="100%" cellpadding="0" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddFileSystemButton" runat="server" SkinID="AddButton" OnClick="AddFileSystemButton_Click"/>
                                        <ccUI:ToolbarButton ID="EditFileSystemButton" runat="server" SkinID="EditButton" OnClick="EditFileSystemButton_Click"/>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:FileSystemsGridView ID="FileSystemsGridView1" runat="server"  Height="500px"/></td></tr>
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
