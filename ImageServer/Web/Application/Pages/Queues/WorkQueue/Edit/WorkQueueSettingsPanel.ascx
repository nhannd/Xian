<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSettingsPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueSettingsPanel" %>

<asp:UpdatePanel ID="WorkQueueSettingsUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <table width="100%">
        <tr>
            <td valign="bottom" align="left" width="75%">
                <asp:Label ID="Label2" runat="server" Text="Priority: " CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="DialogDropDownList"></asp:DropDownList>
            </td>
            <td valign="bottom">
                            <asp:Label ID="Label1" runat="server" Text="Schedule: " CssClass="DialogTextBoxLabel"></asp:Label>
            </td>
            <td valign="bottom" align="left">
                <asp:Label ID="Label4" runat="server" Text="Date" CssClass="DialogTextBoxLabel"></asp:Label><br /><asp:TextBox ID="NewScheduleDate" runat="server" Width="80px" CssClass="DialogTextBox"></asp:TextBox>
                <ccUI:CalendarExtender ID="CalendarExtender" runat="server" TargetControlID="NewScheduleDate" CssClass="Calendar">
                </ccUI:CalendarExtender>
            </td>
            <td valign="bottom" align="left">
                <asp:Label ID="Label5" runat="server" Text="Time" CssClass="DialogTextBoxLabel"></asp:Label><br /><asp:TextBox ID="NewScheduleTime" runat="server" CssClass="DialogTextBox"  ValidationGroup="vg1"/>
                <aspAjax:MaskedEditExtender runat="server" ID="NewScheduleTimeMaskedEditExtender" MaskType="Time" AcceptAMPM="true" TargetControlID="NewScheduleTime" Mask="99:99" MessageValidatorTip="false" OnInvalidCssClass="InvalidTextEntered" />
                <aspAjax:MaskedEditValidator runat="server" ID="NewScheduleTimeMaskedEditValidator" ControlExtender="NewScheduleTimeMaskedEditExtender" ControlToValidate="NewScheduleTime" ValidationExpression="(0[1-9]|1[0-2]):[0-5][0-9] ([ap]m|[AP]M)" ValidationGroup="vg1"  />
            </td>
            <td valign="bottom" nowrap="nowrap" style="padding-left: 10px; padding-right: 10px;">
                <asp:Label ID="Label3" runat="server" Text="Schedule Now:" CssClass="DialogTextBoxLabel"></asp:Label>
                <asp:CheckBox runat="server" ID="ScheduleNowCheckBox" OnCheckedChanged="ScheduleNow_CheckChanged" Checked="false" AutoPostBack="true"/>
            </td>            
        </tr>
    </table>
    </ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox ID="ErrorMessageBox" runat="server" />


