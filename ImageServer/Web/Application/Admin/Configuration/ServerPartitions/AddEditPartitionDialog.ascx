<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.AddEditPartitionDialog" Codebehind="AddEditPartitionDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
    
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="PopupWindow" Style="display: none"
            Width="400px">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="PopupWindowTitleBar" Width="100%">
                <table style="width: 100%">
                    <tr>
                        <td valign="middle">
                            <asp:Label ID="TitleLabel" runat="server" EnableViewState="False" Text="Add Partition"
                                Width="100%"></asp:Label></td>
                    </tr>
                </table>
            </asp:Panel>
            <cc2:validationsummary id="ValidationSummary1" runat="server" backcolor="#FFFFC0"
                height="81px" showmessagebox="false" showsummary="False" style="left: 462px; position: absolute;
                top: 77px" validationgroup="vg1" width="159px">
</cc2:validationsummary>
            <div class="PopupWindowBody" style="vertical-align: top; text-align: center">
                <asp:Panel ID="Panel1" runat="server" CssClass="Panel">
                    <table id="Table2" runat="server" cellspacing="3" style="text-align: left" width="100%">
                        <tr id="Tr1" runat="server">
                            <td id="Td1" runat="server" valign="bottom">
                                AE Title<br />
                                <asp:TextBox ID="AETitleTextBox" runat="server" MaxLength="16" ValidationGroup="vg1"></asp:TextBox>
                                <clearcanvas:conditionalrequiredfieldvalidator id="RequiredFieldValidator2" runat="server"
                                    controltovalidate="AETitleTextBox" display="None" enableclientscript="true" 
                                    errormessage="AE Title is required!!"
                                    invalidinputbackcolor="#FAFFB5" validationgroup="vg1">
                                 </clearcanvas:conditionalrequiredfieldvalidator>
                                <clearcanvas:regularexpressionfieldvalidator id="RegularExpressionFieldValidator2"
                                    runat="server" controltovalidate="AETitleTextBox" display="None" errormessage="The AE Title is not valid."
                                    invalidinputbackcolor="#FAFFB5" validationexpression="^([^\\]){1,16}$"
                                    validationgroup="vg1">
                                    </clearcanvas:regularexpressionfieldvalidator>
                            </td>
                            <td id="Td2" runat="server" align="left" colspan="1" valign="bottom">
                            </td>
                            <td id="Td3" runat="server" colspan="1" style="color: #000000" valign="bottom">
                                                Description<br />
                                <asp:TextBox ID="DescriptionTextBox" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr id="Tr2" runat="server">
                            <td id="Td4" runat="server" style="height: 29px" valign="bottom">
                                Port<br />
                                <asp:TextBox ID="PortTextBox" runat="server"></asp:TextBox>
                                <clearcanvas:RangeValidator 
                                                ID="PortValidator1" runat="server"
                                                ControlToValidate="PortTextBox"
                                                InvalidInputBackColor="#FAFFB5"
                                                ValidationGroup="vg1" 
                                                MinValue = "0"
                                                MaxValue = "65535"
                                                ErrorMessage="Parition Port must be between 0 and 65535"
                                                Display="None">
                                            </clearcanvas:RangeValidator>
                                
                                </td>
                            <td id="Td5" runat="server" align="left" colspan="1" style="height: 29px" valign="bottom">
                            </td>
                            <td id="Td6" runat="server" align="left" colspan="1" style="height: 29px" valign="bottom">
                                Folder Name<br />
                                <asp:TextBox ID="PartitionFolderTextBox" runat="server" CausesValidation="true" ValidationGroup="vg1"></asp:TextBox>
                                <clearcanvas:ConditionalRequiredFieldValidator ID="Conditionalrequiredfieldvalidator1"
                                    runat="server" ControlToValidate="PartitionFolderTextBox" 
                                    Display="None" EnableClientScript="true"
                                    ErrorMessage="Folder Name is required!!" 
                                    InvalidInputBackColor="#FAFFB5" 
                                    ValidationGroup="vg1">
                                 </clearcanvas:ConditionalRequiredFieldValidator></td>
                        </tr>
                        <tr id="Tr3" runat="server">
                            <td id="Td7" runat="server" style="height: 43px">
                                <asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="True" Text="Enabled" /></td>
                            <td id="Td8" runat="server" style="height: 43px">
                            </td>
                            <td id="Td9" runat="server" style="height: 43px" valign="bottom">
                                </td>
                        </tr>
                    </table>
                </asp:Panel>
                <table style="width: 49%">
                    <tr>
                        <td>
                        </td>
                        <td style="width: 29px">
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="OKButton" runat="server" OnClick="OKButton_Click" Text="Add" ValidationGroup="vg1"
                                Width="77px" /></td>
                        <td align="center" style="width: 29px">
                            </td>
                        <td align="center">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td style="width: 29px">
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Style="z-index: 101; left: 522px;
                    position: absolute; top: 53px" Width="36px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" BackgroundCssClass="modalBackground"
            BehaviorID="MyStupidExtender" Enabled="true" PopupControlID="DialogPanel" TargetControlID="DummyPanel">
        </cc1:ModalPopupExtender>
        <cc1:NumericUpDownExtender ID="PortTextBoxNumericUpDownExtender" runat="server" Maximum="65536"
            Minimum="0" TargetControlID="PortTextBox" Width="100">
        </cc1:NumericUpDownExtender>
    </ContentTemplate>
</asp:UpdatePanel>
&nbsp;
