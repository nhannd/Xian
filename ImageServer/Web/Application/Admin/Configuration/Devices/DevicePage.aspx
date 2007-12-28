<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="DevicePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DevicePage"
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
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent" CssClass="ContentWindow">
                    <asp:UpdatePanel ID="DevicePageUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ccPartitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                            <ccAddEdit:AddEditDeviceDialog ID="AddEditDeviceControl1" runat="server" />
                            <ccConfirm:ConfirmDialog ID="ConfirmDialog1" runat="server" />
                            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
