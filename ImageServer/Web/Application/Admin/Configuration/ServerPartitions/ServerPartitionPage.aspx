<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="ServerPartitionPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPage"
    Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="uc1" %>

<asp:Content ID="LocationName" ContentPlaceHolderID="LocationNamePlaceHolder" runat="server">Configure > Server Partitions</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                <uc1:ServerPartitionPanel ID="ServerPartitionPanel" runat="server"></uc1:ServerPartitionPanel>
            </asp:Panel>
            <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
            <uc1:ConfirmationDialog ID="deleteConfirmBox" runat="server" />       
            <uc1:ConfirmationDialog ID="MessageBox" runat="server" />             
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
