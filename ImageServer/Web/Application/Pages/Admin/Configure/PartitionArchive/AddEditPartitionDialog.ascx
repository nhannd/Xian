<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server" CssClass="DialogPanelContent" style="padding-top: 10px;" DefaultButton="OKButton">
            <table width="100%">
                <tr>
                    <td align="left">
                        <table>
                            <tr>
                                <td><asp:Label ID="Label1" runat="server" Text="Description" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:TextBox ID="Description" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="128" Width="300" CssClass="DialogTextBox"></asp:TextBox></td>
                                <td>
                                    <ccAsp:InvalidInputIndicator ID="DescriptionHelp" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>
                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                         runat="server" ControlToValidate="Description" InvalidInputCSS="DialogTextBoxInvalidInput"                                                        ValidationGroup="AddEditPartitionValidationGroup"
                                         Text="Partition Archive description is a required field" InvalidInputIndicatorID="DescriptionHelp"
                                         Display="None"></ccValidator:ConditionalRequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label2" runat="server" Text="Archive Delay" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:TextBox ID="ArchiveDelay" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="4" Width="30" CssClass="DialogTextBox"></asp:TextBox> Hours
                                </td>
                                <td>                                   
                                <ccAsp:InvalidInputIndicator ID="ArchiveDelayHelp" runat="server" SkinID="InvalidInputIndicator"></ccAsp:InvalidInputIndicator>
                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator2"
                                         runat="server" ControlToValidate="ArchiveDelay" InvalidInputCSS="DialogTextBoxInvalidInput"
                                         ValidationGroup="AddEditPartitionValidationGroup"
                                         Text="Archive Delay is a required field" InvalidInputIndicatorID="ArchiveDelayHelp"
                                         Display="None"></ccValidator:ConditionalRequiredFieldValidator>
                                    <ccValidator:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator1"
                                         runat="server" ControlToValidate="ArchiveDelay" InvalidInputCSS="DialogTextBoxInvalidInput"
                                         IgnoreEmptyValue="true" ValidationGroup="AddEditPartitionValidationGroup" ValidationExpression="^([0-9]+)"
                                         Text="Archive Delay must be numeric" Display="None" InvalidInputIndicatorID="ArchiveDelayHelp"></ccValidator:RegularExpressionFieldValidator>                                         
                                 </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label6" runat="server" Text="Archive Type" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:DropDownList ID="ArchiveTypeDropDownList" runat="server" Width="125" CssClass="DialogDropDownList"/></td>
                                <td>

                                </td>
                            </tr>
                            <tr>
                                <td valign="top"><asp:Label ID="Label3" runat="server" Text="Configuration XML" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:TextBox ID="ConfigurationXML" TextMode="MultiLine" runat="server" ValidationGroup="AddEditPartitionValidationGroup" MaxLength="4" Width="300" CssClass="DialogTextBox" Rows="5"></asp:TextBox></td>
                                <td>                                   
                                    
                                 </td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label4" runat="server" Text="Enabled" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:CheckBox ID="EnabledCheckBox" runat="server" CssClass="DialogCheckBox"
                                                        ToolTip="Enable this partition archive." /></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="Label5" runat="server" Text="Read Only" CssClass="DialogTextBoxLabel" /></td>
                                <td><asp:CheckBox ID="ReadOnlyCheckBox" runat="server" CssClass="DialogCheckBox" ToolTip="Make this partition archive read only." /></td>
                                <td>                                   
                                 </td>
                            </tr>
                         </table>
                     </td>
                 </tr>
                 <tr>
                    <td align="left">
                        <table>
                            <tr>
                                <td>
                                         
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                        </table>
                     </td>
                  </tr>
               </table>
        </asp:Panel>
                    <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" OnClick="OKButton_Click" ValidationGroup="AddEditPartitionValidationGroup" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>

    </ContentTemplate>
</ccAsp:ModalDialog>
