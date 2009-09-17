<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.EditServiceLockDialog"
    Codebehind="EditServiceLockDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Title="Edit Service Schedule" Width="450px">
    <ContentTemplate>
        <asp:Panel runat="server" CssClass="DialogPanelContent" style="padding: 6px;">
            <asp:Table ID="Table1" runat="server">
                <asp:TableRow>
                    <asp:TableCell Width="30%" CssClass="DialogTextBoxLabel">
                        Description
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Description" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel">
                        Type
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Type" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel">
                        File System
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="FileSystem" runat="server" Text="Label" CssClass="DialogLabel"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel">
                        Enabled
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:CheckBox ID="Enabled" runat="server" CssClass="DialogCheckbox" />
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell CssClass="DialogTextBoxLabel">
                        Schedule
                    </asp:TableCell>
                    <asp:TableCell Wrap="false">
                        <asp:TextBox ID="ScheduleDate" runat="server" Width="80px" ReadOnly="true" CssClass="DialogTextBox"></asp:TextBox>
                        <asp:Button ID="DatePickerButton" runat="server" Text="..."/>
                        <ccUI:CalendarExtender ID="CalendarExtender" runat="server" 
                                    TargetControlID="ScheduleDate" PopupButtonID="DatePickerButton" CssClass="Calendar" >
                        </ccUI:CalendarExtender>&nbsp;
                        <asp:TextBox ID="ScheduleTime" runat="server" CssClass="DialogTextBox"  ValidationGroup="AddEditServiceLockValidationGroup"/>
                        <aspAjax:MaskedEditExtender runat="server" ID="ScheduleTimeMaskedEditExtender" MaskType="Time" AcceptAMPM="true" TargetControlID="ScheduleTime" Mask="99:99" MessageValidatorTip="false" OnInvalidCssClass="InvalidTextEntered"/>
                        <aspAjax:MaskedEditValidator runat="server" ID="ScheduleTimeMaskedEditValidator" ControlExtender="ScheduleTimeMaskedEditExtender" ControlToValidate="ScheduleTime" ValidationExpression="(0[1-9]|1[0-2]):[0-5][0-9] ([ap]m|[AP]M)" ValidationGroup="AddEditServiceLockValidationGroup"  />
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow><asp:TableCell><asp:image runat="server" SkinID="Spacer" height="3" /></asp:TableCell></asp:TableRow>
            </asp:Table>
</asp:Panel>
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="ApplyButton" OnClick="OKButton_Click" ValidationGroup="AddEditServiceLockValidationGroup" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</ccAsp:ModalDialog>

<ccAsp:MessageBox ID="ErrorMessageBox" runat="server" />