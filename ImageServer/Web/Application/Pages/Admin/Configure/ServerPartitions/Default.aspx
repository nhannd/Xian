<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerPartitions.Default"
    Title="Configure Server Partitions" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Server Partitions</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <uc1:ServerPartitionPanel ID="ServerPartitionPanel" runat="server"></uc1:ServerPartitionPanel>
            </asp:Panel>      
            <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog" runat="server" /> 
        </ContentTemplate>
    </asp:UpdatePanel>

    <ccAsp:MessageBox ID="deleteConfirmBox" runat="server" />       
   <ccAsp:MessageBox ID="MessageBox" runat="server" />     
            <ccAsp:TimedDialog ID="TimedDialog" runat="server" Timeout="3500" /> 
</asp:Content>


