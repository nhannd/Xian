<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ChangePasswordDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login.ChangePasswordDialog" %>

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="500px" Title="Change Password">
    <ContentTemplate>
    
    <asp:Panel runat="server" Visible="false" ID="ErrorMessagePanel" CssClass="ErrorMessage" style="margin-bottom: 10px;">
        <asp:Label runat="server" ID="ErrorMessage" ></asp:Label>
    </asp:Panel>
    
    <asp:Panel ID="Panel1" runat="server" width="100%" CssClass="DialogPanelContent">
    
        <table style="margin-top: 10px; margin-bottom: 10px;">
        <tr><td class="ChangePasswordLabel">Username:</td><td><asp:TextBox runat="server" Width="150px" ID="ChangePasswordUsername"/></td></tr>
        <tr><td class="ChangePasswordLabel">Original Password:</td><td><asp:TextBox TextMode="Password" runat="server" Width="150px" ID="OriginalPassword"/></td></tr>
        <tr><td class="ChangePasswordLabel">New Password:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="NewPassword"/></td></tr>
        <tr><td class="ChangePasswordLabel">Retype New Password:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="ConfirmNewPassword"/></td></tr>
        <tr><td colspan="2" style="padding-top: 10px; font-family: Arial; font-size: 12px;" align="right">Login after password change: <asp:CheckBox runat="server" Checked="true" id="LoginPasswordChange"/></td></tr>
        </table>
           
    </asp:Panel>
    
<table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" OnClick="ChangePassword_Click" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="Cancel_Click"/>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    
    </ContentTemplate>
</ccAsp:ModalDialog>


