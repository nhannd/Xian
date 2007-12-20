<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchFilterPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ccAjax" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel">
    <table cellpadding="2" cellspacing="0">
        <tr>
            <td align="left" valign="bottom">
                <asp:Label ID="Label2" runat="server" Text="Patient ID" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="PatientId" runat="server" Width="100px" ToolTip="Filter the list by Patient Id" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label3" runat="server" Text="Accession" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="AccessionNumber" runat="server" Width="100px" ToolTip="Filter the list by Accession Number" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label4" runat="server" Text="Description" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="StudyDescription" runat="server" Width="100px" ToolTip="Filter the list by Study Description" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label1" runat="server" Text="Schedule" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="ScheduleDate" runat="server" Width="100px" ToolTip="Filter the list by Scheduled Date" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label5" runat="server" Text="Type" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:DropDownList ID="TypeDropDownList" runat="server">
                </asp:DropDownList></td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label6" runat="server" Text="Status" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:DropDownList ID="StatusDropDownList" runat="server">
                </asp:DropDownList></td>
            <td align="right" valign="bottom" style="width: 57px">
                <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                    OnClick="FilterButton_Click" ToolTip="Filter" />
            </td>
        </tr>
    </table>
</asp:Panel>
<ccAjax:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="ScheduleDate" CssClass="Calendar">
</ccAjax:CalendarExtender>
