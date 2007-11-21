<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false"
    CodeBehind="FileSystemsPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsPage" Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditFileSystemDialog.ascx" TagName="AddEditFileSystemDialog"
    TagPrefix="uc3" %>


<%@ Register Src="FileSystemsPanel.ascx" TagName="FileSystemsPanel" TagPrefix="uc2" %>

<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc1" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="WindowTitleBar">
            File System Management
            </td>
         </tr>
         <tr>
            <td>
                <asp:Panel ID="Panel1" runat="server" CssClass="ChildPanel">
                    
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <uc2:FileSystemsPanel id="FileSystemsPanel1" runat="server">
                        </uc2:FileSystemsPanel>
                    <uc3:AddEditFileSystemDialog ID="AddEditFileSystemDialog1" runat="server" />
                    
                    </ContentTemplate>
                </asp:UpdatePanel>
                  
                </asp:Panel>
       
            </td>
         </tr>
         
    </table>
    

</asp:Content>
