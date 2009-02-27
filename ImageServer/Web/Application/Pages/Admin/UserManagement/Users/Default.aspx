<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.Default" Title="Users | User Management | ClearCanvas ImageServer"%>

<%@ Register Src="UserPanel.ascx" TagName="UserPanel" TagPrefix="localAsp" %>
<%@ Register Src="~/Pages/Admin/UserManagement/Users/AddEditUserDialog.ascx" TagName="AddEditUserDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">User Management</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
               <localAsp:UserPanel runat="server" ID="UserPanel" />              
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:AddEditUserDialog ID="AddEditUserDialog" runat="server" />
    <ccAsp:MessageBox ID="DeleteConfirmation" runat="server" />   
</asp:Content>