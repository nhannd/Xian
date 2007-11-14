<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileSystemsPanel.ascx.cs" 
        Inherits="ImageServerWebApplication.Admin.Configuration.FileSystems.FileSystemsPanel" %>
<%@ Register Src="FileSystemsFilterPanel.ascx" TagName="FileSystemsFilterPanel" TagPrefix="uc4" %>
<%@ Register Src="FileSystemsToolBar.ascx" TagName="FileSystemsToolBar" TagPrefix="uc2" %>
<%@ Register Src="FileSystemsGridView.ascx" TagName="FileSystemsGridView" TagPrefix="uc1" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server"  style="" >

            <table cellpadding="0" cellspacing="0" width="100%" >
                <tr class="toolBarPanel">
                    <td colspan="1" valign="bottom">
                       <uc2:FileSystemsToolBar ID="FileSystemsToolBar1" runat="server" />
                
                    </td>
                    <td align="right" valign="bottom">
                     <uc4:FileSystemsFilterPanel id="FileSystemsFilterPanel1" runat="server">
                </uc4:FileSystemsFilterPanel>
                        </td>
                </tr>
                <tr class="toolBarPanel">
                    <td colspan="2">
                    </td>
                </tr>
                <tr>
                    <td colspan="2" valign="top">
                    <uc1:FileSystemsGridView id="FileSystemsGridView1" runat="server">
                </uc1:FileSystemsGridView>
                        </td>
                </tr>
                <tr>
                    <td colspan="2">
                    <uc8:GridPager id="GridPager1" runat="server">
                </uc8:GridPager>
                        </td>
                </tr>
            </table>
           
        </asp:Panel>
    </ContentTemplate>
    
</asp:UpdatePanel>
                
                
               

