<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.Default"
     %>

<%@ Register Src="ServiceLockPanel.ascx" TagName="ServiceLockPanel" TagPrefix="localAsp" %>

<asp:Content ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,ServiceScheduling%>" /></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <localAsp:ServiceLockPanel ID="ServiceLockPanel" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    

</asp:Content>
