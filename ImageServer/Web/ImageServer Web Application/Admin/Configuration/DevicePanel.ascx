<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DevicePanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.DevicePanel" %>
<%@ Register Src="../../Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>

<%@ Register Src="EditDeviceDialog.ascx" TagName="EditDeviceDialog" TagPrefix="uc7" %>
<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc6" %>
<%@ Register Src="DeviceGridViewPager.ascx" TagName="DeviceGridViewPager" TagPrefix="uc4" %>
<%@ Register Src="DeviceToolBar.ascx" TagName="DeviceToolBar" TagPrefix="uc2" %>
<%@ Register Src="DeviceFilterPanel.ascx" TagName="DeviceFilterPanel" TagPrefix="uc3" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="uc1" %>
<asp:Panel ID="Panel1" runat="server"  style="padding-right: 20px; padding-left: 20px; padding-bottom: 20px; padding-top: 10px" Height="631px" >

    <table width="100%" cellpadding="0" cellspacing="0" style="height: 683px">
        <tr class="toolBarPanel">
            <td colspan="1">
                <uc2:DeviceToolBar ID="DeviceToolBarControl1" runat="server" />
            </td>
            <td align="right">
               <uc3:DeviceFilterPanel id="DeviceFilterPanel1" runat="server"></uc3:DeviceFilterPanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" valign="top">
                <uc1:DeviceGridView ID="DeviceGridViewControl1" runat="server" />
            </td>
            
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
   
</asp:Panel>
<uc6:AddDeviceDialog ID="AddDeviceControl1" runat="server" />
<uc7:EditDeviceDialog ID="EditDeviceControl1" runat="server" />
<uc5:ConfirmDialog ID="ConfirmDialog1" runat="server" />



