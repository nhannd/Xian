<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DevicePanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.DevicePanel" %>
<%@ Register Src="../../Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Src="../../Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>

<%@ Register Src="EditDeviceDialog.ascx" TagName="EditDeviceDialog" TagPrefix="uc7" %>
<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc6" %>
<%@ Register Src="DeviceToolBar.ascx" TagName="DeviceToolBar" TagPrefix="uc2" %>
<%@ Register Src="DeviceFilterPanel.ascx" TagName="DeviceFilterPanel" TagPrefix="uc3" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="uc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Panel ID="Panel1" runat="server"  style="padding-right: 20px; padding-left: 20px; padding-bottom: 20px; padding-top: 10px" >

    <table cellpadding="0" cellspacing="0" >
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
            <td colspan="2" style="height: 20px">
                <uc8:GridPager id="GridPager1" runat="server">
                </uc8:GridPager></td>
        </tr>
    </table>
   
</asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>


