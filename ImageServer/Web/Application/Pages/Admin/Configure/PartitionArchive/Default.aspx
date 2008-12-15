<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" ValidateRequest="false"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.Default"
    Title="Configure Partition Archive" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="localAsp" %>
<%@ Register Src="PartitionArchivePanel.ascx" TagName="PartitionArchivePanel" TagPrefix="localAsp" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Partition Archive</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <localAsp:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
    <ccAsp:MessageBox ID="DeleteConfirmDialog" runat="server" />       
    <ccAsp:MessageBox ID="MessageBox" runat="server" />     
    <ccAsp:TimedDialog ID="TimedDialog" runat="server" Timeout="3500" />        
</asp:Content>
