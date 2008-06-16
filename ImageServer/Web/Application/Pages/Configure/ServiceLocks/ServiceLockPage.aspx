<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="ServiceLockPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServiceLocks.ServiceLockPage"
    Title="Configure Service Scheduling" %>

<%@ Register Src="ServiceLockPanel.ascx" TagName="ServiceLockPanel" TagPrefix="localAsp" %>

<asp:Content ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Service Scheduling</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <localAsp:ServiceLockPanel ID="ServiceLockPanel" runat="server" />
            <ccAsp:ConfirmationDialog ID="ConfirmationDialog1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    
</asp:Content>
