<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" Inherits="Admin_Configuration_Devices" Codebehind="Devices.aspx.cs" %>

<%@ Register Src="DeviceToolBarControl.ascx" TagName="DeviceToolBarControl" TagPrefix="uc3" %>
<%@ Register Src="DeviceFilterControl.ascx" TagName="DeviceFilterControl" TagPrefix="uc6" %>
<%@ Register Src="EditDeviceControl.ascx" TagName="EditDeviceControl" TagPrefix="uc5" %>
<%@ Register Src="AddDeviceControl.ascx" TagName="AddDeviceControl" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="DeviceGridViewControl.ascx" TagName="DeviceGridViewControl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <table style="width: 100%">
                    <tr>
                        <td valign="top">
                            <asp:Panel ID="Panel3" runat="server" Height="439px">
                                <table>
                                    <tr>
                                        <td style="font-weight: bold; height: 50px" class="WindowTitleBar">
                                            DEVICE MANAGEMENT</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc3:DeviceToolBarControl ID="DeviceToolBarControl1" runat="server" OnLoad="DeviceToolBarControl1_Load"></uc3:DeviceToolBarControl>
                                            <uc6:DeviceFilterControl ID="DeviceFilterControl1" runat="server"></uc6:DeviceFilterControl>
                                            <br />
                                            <uc1:DeviceGridViewControl ID="DeviceGridViewControl1" runat="server"></uc1:DeviceGridViewControl>
                                    </tr>
                            </asp:Panel>
                            </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>

    <uc2:AddDeviceControl ID="AddDeviceControl1" runat="server" />
    <uc5:EditDeviceControl ID="EditDeviceControl1" runat="server" />
</asp:Content>
