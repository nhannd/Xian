<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchFilterPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchFilterPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ccAjax" %>
<asp:Panel ID="Panel1" runat="server" CssClass="toolBarPanel">
    <table cellpadding="2" cellspacing="0">
        <tr>
            <td align="left" valign="bottom">
                <asp:Label ID="Label1" runat="server" Text="Patient Name" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="PatientName" runat="server" Width="100px" ToolTip="Filter the list by Patient Name" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label2" runat="server" Text="Patient ID" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="PatientId" runat="server" Width="100px" ToolTip="Filter the list by Patient Id" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label3" runat="server" Text="Accession Number" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="AccessionNumber" runat="server" Width="100px" ToolTip="Filter the list by Accession Number" />
            </td>
            <td align="left" valign="bottom">
                <asp:Label ID="Label4" runat="server" Text="Description" Width="68px" Style="padding-right: 5px"
                    EnableViewState="False" /><br />
                <asp:TextBox ID="StudyDescription" runat="server" Width="100px" ToolTip="Filter the list by Study Description" />
            </td>
            <td align="right" valign="bottom" style="width: 57px">
                <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                    OnClick="FilterButton_Click" ToolTip="Filter" />
            </td>
        </tr>
    </table>
</asp:Panel>
