<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PartitionArchivePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchivePanel" %>

<%@ Register Src="PartitionArchiveGridPanel.ascx" TagName="PartitionArchiveGridPanel" TagPrefix="localAsp" %>

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
                                                <asp:Label ID="Label2" runat="server" Text="Description" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox"></asp:TextBox>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Type" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="ArchiveTypeFilter" runat="server" CssClass="SearchDropDownList"></asp:DropDownList>
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Status" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList"></asp:DropDownList>
                                            </td>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><asp:ImageButton ID="SearchButton" runat="server" SkinID="SearchButton" OnClick="SearchButton_Click"/></asp:Panel>
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
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddPartitionButton" runat="server" SkinID="AddButton" OnClick="AddPartitionButton_Click" />
                                        <ccUI:ToolbarButton ID="EditPartitionButton" runat="server" SkinID="EditButton" OnClick="EditPartitionButton_Click" />
                                        <ccUI:ToolbarButton ID="DeletePartitionButton" runat="server" SkinID="DeleteButton" OnClick="DeletePartitionButton_Click" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:PartitionArchiveGridPanel ID="PartitionArchiveGridPanel" runat="server" Height="500px" /></td></tr>
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
