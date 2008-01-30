<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="DevicePage.aspx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DevicePage"
    Title="ImageServer Device Config" %>

<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs"
    TagPrefix="ccPartitionTabs" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="ccConfirm" %>
<%@ Register Src="AddEditDeviceDialog.ascx" TagName="AddEditDeviceDialog" TagPrefix="ccAddEdit" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Device Management
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ccPartitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
            <ccAddEdit:AddEditDeviceDialog ID="AddEditDeviceControl1" runat="server" />
            <ccConfirm:ConfirmDialog ID="ConfirmDialog1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
</asp:Content>
