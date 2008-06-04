<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.FileSystems.FileSystemsPanel" %>

<%@ Register Src="FileSystemsGridView.ascx" TagName="FileSystemsGridView" TagPrefix="localAsp" %>
    
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

<div>
        <b class="roundedCorners"><b class="roundedCorners1"><b></b></b><b class="roundedCorners2">
            <b></b></b><b class="roundedCorners3"></b><b class="roundedCorners4"></b><b class="roundedCorners5">
            </b></b>
        <div class="roundedCornersfg">          


            <asp:Table ID="Table" runat="server" Width="100%">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchToolbarButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="Description" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search by description"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="Tiers" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="SearchButtonContainer" runat="server" CssClass="SearchButtonContainer">
                                                   <asp:ImageButton ID="SearchToolbarButton" runat="server" SkinID="SearchButton" OnClick="SearchButton_Click" Tooltip="Search for File Systems" />
                                                 </asp:Panel>
                                            </td>
                                
                                        </tr> 
                                    </table>
                            </asp:Panel>
                    </asp:TableCell> 
                 </asp:TableRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
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
       
        </div>
        <b class="roundedCorners"><b class="roundedCorners5"></b><b class="roundedCorners4">
        </b><b class="roundedCorners3"></b><b class="roundedCorners2"><b></b></b><b class="roundedCorners1">
            <b></b></b></b>
    </div>       
    </ContentTemplate>
</asp:UpdatePanel>
