<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServiceLocks.Default"
    Title="Configure Service Scheduling" %>

<%@ Register Src="ServiceLockPanel.ascx" TagName="ServiceLockPanel" TagPrefix="localAsp" %>
<%@ Register Src="EditServiceLockDialog.ascx" TagName="EditServiceLockDialog" TagPrefix="localAsp" %>

<asp:Content ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Service Scheduling</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <localAsp:ServiceLockPanel ID="ServiceLockPanel" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <ccAsp:MessageBox ID="ConfirmEditDialog" runat="server" />
    <localAsp:EditServiceLockDialog ID="EditServiceLockDialog" runat="server" />
</asp:Content>
