<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSettingsPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.WorkQueueSettingsPanel" %>

<asp:Panel ID="Panel1" runat="server" style="padding:5px; text-align:center">
    <table width="100%">
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Priority: "></asp:Label>
                
            </td>
            <td>
                <asp:DropDownList ID="PriorityDropDownList" runat="server" Width="100px"></asp:DropDownList>
            </td>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Schedule: "></asp:Label>
                <asp:TextBox ID="NewScheduleDate" runat="server" Width="80px"></asp:TextBox>
                <asp:Button ID="DatePickerButton" runat="server" Text="..." />
                <aspAjax:CalendarExtender ID="CalendarExtender" runat="server" TargetControlID="NewScheduleDate" PopupButtonID="DatePickerButton" CssClass="Calendar">
                </aspAjax:CalendarExtender>
                
                <asp:DropDownList ID="NewScheduleTimeDropDownList" runat="server">
                    
                </asp:DropDownList>
            </td>
            
        </tr>
    </table>
</asp:Panel>
