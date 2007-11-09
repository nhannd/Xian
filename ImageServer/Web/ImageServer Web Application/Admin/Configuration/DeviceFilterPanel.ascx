<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceFilterPanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.DeviceFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel"  >
<table cellpadding="0" cellspacing="0" >
    <tr valign="middle">
        <td  align="right" style="display: block; overflow: visible;">
            <asp:Label ID="Label1" runat="server" Text="AE Title" Width="68px" style="padding-right: 5px" EnableViewState="False"></asp:Label></td>
        <td ><asp:TextBox ID="AETitleFilter" runat="server" Width="81px" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
        <td  align="right">
            <asp:Label ID="Label2" runat="server" Text="IP" Width="31px" style="padding-right: 5px" EnableViewState="False"></asp:Label></td>
        <td  >
        <asp:TextBox ID="IPAddressFilter" runat="server" Width="92px" ToolTip="Filter the list by IP Address"></asp:TextBox></td>
        <td  align="right" >
            <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Active" TextAlign="Left"
                Width="80px" style="padding-right: 5px; padding-left: 5px" ToolTip="Show active devices only" /></td>
        <td  align="right" >
            <asp:CheckBox ID="DHCPOnlyFilter" runat="server" Text="DHCP" TextAlign="Left" Width="76px" style="padding-right: 5px; padding-left: 5px" ToolTip="Show only devices using DHCP" /></td>
        <td style="width: 80px;" align="right">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/filter.gif" OnClick="FilterButton_Click" ToolTip="Filter" /></td>
    </tr>
   
    
</table>
</asp:Panel>

