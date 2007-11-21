<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceFilterPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DeviceFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel"  >
<table cellpadding="3" cellspacing="0" >
    <tr >
        <td  align="left">
            <asp:Label ID="Label1" runat="server" Text="AE Title" Width="68px" EnableViewState="False"></asp:Label><br />
            <asp:TextBox ID="AETitleFilter" runat="server" Width="100px" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
        <td align="left"  >
            <asp:Label ID="Label2" runat="server" Text="IP Address"  EnableViewState="False"></asp:Label><br />
        <asp:TextBox ID="IPAddressFilter" runat="server" Width="100px" ToolTip="Filter the list by IP Address"></asp:TextBox></td>
        <td  align="left" valign="bottom" >
            </td>
        <td  align="left" valign="bottom" >
            <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Active" ToolTip="Show active devices only" /></td>
        <td  align="left" valign="bottom" >
            <asp:CheckBox ID="DHCPOnlyFilter" runat="server" Text="DHCP"  ToolTip="Show only devices using DHCP" /></td>
        <td style="width: 80px;" align="right">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/filter.gif" OnClick="FilterButton_Click" ToolTip="Filter" /></td>
    </tr>
   
    
</table>
</asp:Panel>

