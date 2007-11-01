<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin_Configuration_DeviceFilterControl" Codebehind="DeviceFilterControl.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel" Width="100%">
    <table style="width: 600px; height: 100%;" cellpadding="4">
        <tr>
            <td style="height: 34px">
                Server Partition&nbsp;
    <asp:DropDownList ID="ServerPartitionDropDownList" runat="server" OnSelectedIndexChanged="ServerPartitionDropDownList_SelectedIndexChanged1" Width="169px" AutoPostBack="True"
        ForeColor="Gray">
    </asp:DropDownList></td>
            <td style="width: 169px; height: 34px">
    <asp:CheckBox ID="ActiveOnlyCheckBox" runat="server" OnCheckedChanged="ActiveOnlyCheckBox_CheckedChanged" Text="Active Only" TextAlign="Left" Visible="False" /></td>
        </tr>
    </table>
</asp:Panel>
