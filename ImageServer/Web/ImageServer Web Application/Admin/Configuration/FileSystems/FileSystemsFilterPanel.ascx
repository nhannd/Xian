<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileSystemsFilterPanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.FileSystems.FileSystemsFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel" Width="404px"  >
<table cellpadding="0" cellspacing="2" >
    <tr valign="middle">
        <td  align="right" style="display: block; overflow: visible; height: 43px;">
            </td>
        <td style="width: 82px; height: 43px;" align="left" >
            <asp:Label ID="Label1" runat="server" Text="Description" Width="68px" style="padding-right: 5px" EnableViewState="False"></asp:Label><br />
            <asp:TextBox ID="DescriptionFilter" runat="server" Width="137px" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
        <td  align="right" style="height: 43px">
            </td>
        <td style="height: 43px"  >
        </td>
        <td  style="width: 79px; height: 43px;" align="left" >
            Tiers<br />
            <asp:DropDownList ID="TiersDropDownList" runat="server" Width="131px">
            </asp:DropDownList></td>
        <td style="width: 67px; height: 43px;" valign="bottom"  >
            </td>
        <td valign="bottom" style="height: 43px" >
            &nbsp;</td>
        <td align="right" valign="bottom">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/filter.gif" OnClick="FilterButton_Click" ToolTip="Filter" /></td>
    </tr>
   
    
</table>
</asp:Panel>

