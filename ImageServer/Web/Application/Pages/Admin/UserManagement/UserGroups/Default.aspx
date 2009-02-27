<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.Default" Title="Users | Admin | ClearCanvas ImageServer"%>

<%@ Register Src="UserGroupsPanel.ascx" TagName="UserGroupsPanel" TagPrefix="localAsp" %>
<%@ Register Src="~/Pages/Admin/UserManagement/UserGroups/AddEditUserGroupsDialog.ascx" TagName="AddEditUserGroupsDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">User Group Management</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
               <localAsp:UserGroupsPanel runat="server" ID="UserGroupsPanel" />              
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:AddEditUserGroupsDialog ID="AddEditUserGroupsDialog" runat="server" />
    <ccAsp:MessageBox ID="DeleteConfirmation" runat="server" />    
</asp:Content>