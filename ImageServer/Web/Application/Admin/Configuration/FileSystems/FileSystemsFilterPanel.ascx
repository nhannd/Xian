<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileSystemsFilterPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel">
<table cellpadding="3" cellspacing="0" >
    <tr >
        <td align="left" >
            <asp:Label ID="Label1" runat="server" Text="Description" Width="68px" style="padding-right: 5px" EnableViewState="False"></asp:Label><br />
            <asp:TextBox ID="DescriptionFilter" runat="server" Width="100px" ToolTip="Filter the list by AE Title"></asp:TextBox></td>
        <td  align="left" >
            Tiers<br />
            <asp:DropDownList ID="TiersDropDownList" runat="server" Width="100px">
            </asp:DropDownList></td>
        <td align="right" valign="bottom" style="width: 57px">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png" OnClick="FilterButton_Click" ToolTip="Filter" /></td>
    </tr>
   
    
</table>
</asp:Panel>

