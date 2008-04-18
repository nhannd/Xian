<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="ServerPartitionPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPage"
    Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditPartitionDialog.ascx" TagName="AddEditPartitionDialog" TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionPanel.ascx" TagName="ServerPartitionPanel" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Server Partition Management
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                <uc1:ServerPartitionPanel ID="ServerPartitionPanel" runat="server"></uc1:ServerPartitionPanel>
            </asp:Panel>
            <uc2:AddEditPartitionDialog ID="AddEditPartitionDialog1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
