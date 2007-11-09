<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionFilterPanel.ascx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.ServerPartitions.ServerPartitionFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel" width="100%" >
<table cellpadding="0" cellspacing="0" width="100%" >
    <tr valign="middle">
        <td  align="right" style="display: block; overflow: visible;">
            <asp:Label ID="Label1" runat="server" Text="AE Title" Width="55px" style="padding-right: 5px"></asp:Label>
            <asp:TextBox ID="AETitleFilter" runat="server" Width="90px"></asp:TextBox>
        </td>
        <td  align="right">
            <asp:Label ID="Label2" runat="server" Text="Description" Width="71px" style="padding-right: 5px"></asp:Label>
            <asp:TextBox ID="DescriptionFilter" runat="server" Width="90px"></asp:TextBox>
        </td>
        <td  align="right">
            <asp:Label ID="Label3" runat="server" Text="Folder" Width="49px" style="padding-right: 5px"></asp:Label>
            <asp:TextBox ID="FolderFilter" runat="server" Width="90px"></asp:TextBox>
        </td>
        <td  align="right" >
            <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled" TextAlign="Left"
                Width="92px" />
        </td>
        <td style="width: 60px;" align="right">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/filter.gif" OnClick="FilterButton_Click" />
        </td>
    </tr>
   
    
</table>
</asp:Panel>

