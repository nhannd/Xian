<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceFilterPanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.DeviceFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel"  >
<table cellpadding="0" cellspacing="0" >
    <tr valign="middle">
        <td  align="right" style="display: block; overflow: visible;">
            <asp:Label ID="Label1" runat="server" Text="AE Title" Width="86px" style="padding-right: 5px"></asp:Label></td>
        <td ><asp:TextBox ID="AETitleFilter" runat="server" Width="147px"></asp:TextBox></td>
        <td  align="right">
            <asp:Label ID="Label2" runat="server" Text="IP Address" Width="107px" style="padding-right: 5px"></asp:Label></td>
        <td  >
        <asp:TextBox ID="IPAddressFilter" runat="server" Width="147px"></asp:TextBox></td>
        <td  align="right" >
            <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Active Only" TextAlign="Left"
                Width="130px" /></td>
        <td  align="right" >
            <asp:CheckBox ID="DHCPOnlyFilter" runat="server" Text="DHCP Only" TextAlign="Left" Width="130px" /></td>
        <td style="width: 80px;" align="right">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/filter.gif" OnClick="FilterButton_Click" /></td>
    </tr>
   
    
</table>
</asp:Panel>

