<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.Default"
    Title="Configure Service Scheduling" %>

<%@ Register Src="ServiceLockPanel.ascx" TagName="ServiceLockPanel" TagPrefix="localAsp" %>

<asp:Content ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Service Scheduling</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <localAsp:ServiceLockPanel ID="ServiceLockPanel" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    

</asp:Content>
