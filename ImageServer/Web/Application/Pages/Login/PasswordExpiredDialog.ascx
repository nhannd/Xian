<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PasswordExpiredDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Login.PasswordExpiredDialog" %>

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="500px" Title="Password Expired">
    <ContentTemplate>
    
    <asp:Panel runat="server" Visible="false" ID="ErrorMessagePanel" CssClass="ErrorMessage" style="margin-bottom: 10px;">
        <asp:Label runat="server" ID="ErrorMessage" ></asp:Label>
    </asp:Panel>
    
    <asp:Panel ID="Panel1" runat="server" width="100%" CssClass="DialogPanelContent">

        <asp:Panel runat="server" CssClass="PasswordExpiredMessage"><asp:Label runat="server" ID="Label1" Text="Your password has expired, or this is your first login to the ImageServer. Please enter a new password below." /></asp:Panel>
    
        <table style="margin-top: 10px; margin-bottom: 10px;">
        <tr><td class="ChangePasswordLabel">Username:</td><td><asp:TextBox runat="server" Width="150px" ID="Username"/></td></tr>
        <tr><td class="ChangePasswordLabel">New Password:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="NewPassword"/></td></tr>
        <tr><td class="ChangePasswordLabel">Retype New Password:</td><td><asp:TextBox TextMode="Password" runat="server"  Width="150px" ID="ConfirmNewPassword"/></td></tr>
        </table>
        
        <input type="hidden" runat="server" id="OriginalPassword" />
           
    </asp:Panel>
    
<table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OKButton" OnClick="ChangePassword_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    
    </ContentTemplate>
</ccAsp:ModalDialog>


