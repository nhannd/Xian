<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false"
    CodeBehind="ServerPartitionPage.aspx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.ServerPartitions.ServerPartitionPage" Title="ClearCanvas ImageServer" %>

<%@ Register Src="EditPartitionDialog.ascx" TagName="EditPartitionDialog" TagPrefix="uc3" %>

<%@ Register Src="AddPartitionDialog.ascx" TagName="AddPartitionDialog" TagPrefix="uc2" %>

<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
    <table cellpadding="0" cellspacing="0">
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
    <uc2:AddPartitionDialog ID="AddPartitionDialog" runat="server" />
                </td>
         </tr>
         
    </table>
            <uc3:EditPartitionDialog ID="EditPartitionDialog" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    

</asp:Content>
