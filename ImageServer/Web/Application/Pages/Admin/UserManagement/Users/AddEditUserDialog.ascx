<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.AddEditUserDialog"
    Codebehind="AddEditUserDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="450px">
    <ContentTemplate>
    
        <div class="DialogPanelContent">
    
        <asp:Table runat="server" skinID="NoSkin" CellSpacing="3" CellPadding="3">
       
            <asp:TableRow runat="server" ID="UserNameRow">
                <asp:TableCell runat="server"><asp:Label ID="Label2" runat="server" Text="Username" CssClass="DialogTextBoxLabel" /></asp:TableCell><asp:TableCell><asp:TextBox runat="server" ID="UserLoginId" CssClass="DialogTextBox"></asp:TextBox></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="left" width="100%">
                    <ccAsp:InvalidInputIndicator ID="UserLoginHelpId" runat="server" SkinID="InvalidInputIndicator" />
                    <ccValidator:ConditionalRequiredFieldValidator ID="UserNameRequiredFieldValidator" runat="server"
                                                        ControlToValidate="UserLoginId" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="UserLoginHelpId" Text="Username is required" Display="None"
                                                        RequiredWhenChecked="False"/>
                </asp:TableCell>
            </asp:TableRow>
            
            
            <asp:TableRow>
                <asp:TableCell CssClass="DialogTextBoxLabel" Wrap="false"><asp:Label ID="Label1" runat="server" Text="Display Name" CssClass="DialogTextBoxLabel" /></asp:TableCell><asp:TableCell><asp:TextBox runat="server" ID="DisplayName" CssClass="DialogTextBox"></asp:TextBox></asp:TableCell>
                <asp:TableCell HorizontalAlign="left" width="100%">
                    <ccAsp:InvalidInputIndicator ID="UserDisplayNameHelp" runat="server" SkinID="InvalidInputIndicator" />
                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1" runat="server"
                                                        ControlToValidate="DisplayName" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        InvalidInputIndicatorID="UserDisplayNameHelp" Text="User display name is required" Display="None"
                                                        RequiredWhenChecked="False"/>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow><asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label3" runat="server" Text="Groups" CssClass="DialogTextBoxLabel" /></asp:TableCell><asp:TableCell ColumnSpan="2">
            <div  class="DialogCheckBoxList">
            <asp:CheckBoxList runat="server" ID="UserGroupListBox" />
            </div>
            </asp:TableCell>
            </asp:TableRow>
            
            <asp:TableRow ID="EnabledRow"><asp:TableCell VerticalAlign="top" CssClass="DialogTextBoxLabel"><asp:Label ID="Label4" runat="server" Text="Enabled" CssClass="DialogTextBoxLabel" /></asp:TableCell><asp:TableCell ColumnSpan="2"><asp:CheckBox runat="server" ID="UserEnabledCheckbox" /></asp:TableCell></asp:TableRow>
        </asp:Table>
    
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
