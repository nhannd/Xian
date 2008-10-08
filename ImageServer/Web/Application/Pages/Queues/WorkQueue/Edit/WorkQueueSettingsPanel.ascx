<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSettingsPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueSettingsPanel" %>


    <table>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Priority: " CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="DialogDropDownList"></asp:DropDownList>
            </td>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Schedule: " CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:TextBox ID="NewScheduleDate" runat="server" Width="80px" CssClass="DialogTextBox"></asp:TextBox>
                <ccUI:CalendarExtender ID="CalendarExtender" runat="server" TargetControlID="NewScheduleDate" CssClass="Calendar">
                </ccUI:CalendarExtender>
                <asp:DropDownList ID="NewScheduleTimeDropDownList" runat="server" CssClass="DialogDropDownList" />
            </td>            
        </tr>
    </table>

