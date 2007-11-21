<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false"
    CodeBehind="ServerPartitionPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPage" Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>

<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="WindowTitleBar">
            Server Partition Management
            </td>
         </tr>
         <tr>
            <td>
                <asp:Panel ID="Panel1" runat="server" CssClass="ChildPanel">
                    <uc1:ServerPartitionPanel id="ServerPartitionPanel" runat="server">
                    </uc1:ServerPartitionPanel>
                </asp:Panel>
    <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog1" runat="server" />
                </td>
         </tr>
         
    </table>
            
        </ContentTemplate>
    </asp:UpdatePanel>
    

</asp:Content>
