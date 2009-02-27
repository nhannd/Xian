<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.AddEditUserGroupsDialog"
    Codebehind="AddEditUserGroupsDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="750px">
    <ContentTemplate>
    
        <div class="DialogPanelContent">
    
        <table cellpadding="5">           
            <tr>
                <td class="DialogTextBoxLabel" nowrap="nowrap"><asp:Label ID="Label1" runat="server" Text="Group Name" CssClass="DialogTextBoxLabel" /></td><td><asp:TextBox runat="server" ID="GroupName" CssClass="DialogTextBox"></asp:TextBox></td>
                <td width="100%">
                    <ccAsp:InvalidInputIndicator ID="GroupNameHelpId" runat="server" SkinID="InvalidInputIndicator" />
                    <ccValidator:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="GroupName" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="GroupNameHelpId" Text="Group name is required" Display="None"
                                                        RequiredWhenChecked="False"/>
                </td>
            </tr>
            <tr>
                <td valign="top" class="DialogTextBoxLabel"><asp:Label ID="Label3" runat="server" Text="Tokens" CssClass="DialogTextBoxLabel" /></td>
                <td valign="top" colspan="2">                    
                    <div  class="DialogCheckBoxList">
                       <asp:CheckBoxList id="TokenCheckBoxList" runat="server" TextAlign="Right" RepeatColumns="1"></asp:CheckBoxList>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">

                </td>
            </tr>
            
        </table>
    
        </div>    

        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" ValidationGroup="vg1" OnClick="OKButton_Click"/>
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
