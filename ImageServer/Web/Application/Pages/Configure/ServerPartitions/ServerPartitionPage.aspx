<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="ServerPartitionPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerPartitions.ServerPartitionPage"
    Title="Configure Server Partitions" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Server Partitions</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel" >
                <uc1:ServerPartitionPanel ID="ServerPartitionPanel" runat="server"></uc1:ServerPartitionPanel>
            </asp:Panel>
            <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
            <ccAsp:ConfirmationDialog ID="deleteConfirmBox" runat="server" />       
            <ccAsp:ConfirmationDialog ID="MessageBox" runat="server" />             
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
