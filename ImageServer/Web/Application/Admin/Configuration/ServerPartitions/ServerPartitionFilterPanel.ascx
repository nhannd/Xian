<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionFilterPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel" >
<table cellpadding="3" cellspacing="0" >
    <tr>
        <td  align="left">
            <asp:Label ID="Label1" runat="server" Text="AE Title" Width="55px" ></asp:Label><br />
            <asp:TextBox ID="AETitleFilter" runat="server" Width="100px"></asp:TextBox>
        </td>
        <td  align="left" >
            <asp:Label ID="Label2" runat="server" Text="Description" Width="71px"></asp:Label><br />
            <asp:TextBox ID="DescriptionFilter" runat="server" Width="100px"></asp:TextBox>
        </td>
        <td  align="left" >
            <asp:Label ID="Label3" runat="server" Text="Folder" Width="49px"></asp:Label><br />
            <asp:TextBox ID="FolderFilter" runat="server" Width="100px"></asp:TextBox>
        </td>
        <td  align="left" valign="bottom">
            <asp:CheckBox ID="EnabledOnlyFilter" runat="server" Text="Enabled"
                Width="92px" />
        </td>
        <td style="width: 50px" align="right" valign="bottom">
            <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png" OnClick="FilterButton_Click" />
        </td>
    </tr>
   
    
</table>
</asp:Panel>

