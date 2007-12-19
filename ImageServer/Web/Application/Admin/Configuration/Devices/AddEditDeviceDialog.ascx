<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.AddEditDeviceDialog"
    Codebehind="AddEditDeviceDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="PopupWindow" Width="442px" Style="display: none">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="PopupWindowTitleBar" Width="100%">
                <table style="width: 100%">
                    <tr>
                        <td valign="middle">
                            <asp:Label ID="TitleLabel" runat="server" Text="Add Device" Width="100%" EnableViewState="False"></asp:Label></td>
                    </tr>
                </table>
            </asp:Panel>
            <cc2:ValidationSummary ID="ValidationSummary1" runat="server" Style="left: 42px;
                position: absolute; top: 378px" BackColor="#FFFFC0" Height="81px" ValidationGroup="vg1"
                Width="159px" ShowMessageBox="false" ShowSummary="False" />
            <div class="PopupWindowBody" style="vertical-align: top;">
                <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="150px">
                    <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
                        <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server" CssClass="Panel" Width="100%">
                                <table id="Table2" runat="server" cellspacing="2" style="text-align: left" width="100%">
                                    <tr id="Tr1" runat="server">
                                        <td id="Td1" runat="server" valign="bottom">
                                            AE Title<br />
                                            <asp:TextBox ID="AETitleTextBox" runat="server" ValidationGroup="vg1" MaxLength="16"
                                                Width="150px"></asp:TextBox><asp:Image ID="AETitleHelpImage" runat="server" ImageAlign="Top"
                                                    ImageUrl="~/images/icons/HelpSmall.png" Style="visibility: hidden" /><clearcanvas:ConditionalRequiredFieldValidator
                                                        ID="RequiredFieldValidator2" runat="server" ControlToValidate="AETitleTextBox"
                                                        InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" PopupHelpControlID="AETitleHelpImage"
                                                        ErrorMessage="AE Title is required" Display="None" RequiredWhenChecked="False"></clearcanvas:ConditionalRequiredFieldValidator><clearcanvas:RegularExpressionFieldValidator
                                                            ID="RegularExpressionFieldValidator2" runat="server" ControlToValidate="AETitleTextBox"
                                                            InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" PopupHelpControlID="AETitleHelpImage"
                                                            ValidationExpression="^([^\\]){1,16}$" ErrorMessage="Acceptable characters: any characters except backslash"
                                                            Display="None"></clearcanvas:RegularExpressionFieldValidator>
                                        </td>
                                        <td id="Td2" runat="server" colspan="1" valign="bottom" align="left">
                                        </td>
                                        <td id="Td3" runat="server" colspan="1" style="color: #000000" valign="bottom">
                                            Description<br />
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" Width="150px"></asp:TextBox>&nbsp;
                                        </td>
                                    </tr>
                                    <tr id="Tr2" runat="server">
                                        <td id="Td4" runat="server" valign="bottom">
                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td>
                                                        IP Address
                                                    </td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" Width="68px" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:TextBox ID="IPAddressTextBox" runat="server" CausesValidation="True" ValidationGroup="vg1"
                                                            Width="150px"></asp:TextBox><asp:Image ID="IPAddressHelpImage" runat="server" ImageAlign="Top"
                                                                ImageUrl="~/images/icons/HelpSmall.png" Style="visibility: hidden" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                runat="server" ControlToValidate="IPAddressTextBox" InvalidInputBackColor="#FAFFB5"
                                                ConditionalCheckBoxID="DHCPCheckBox" RequiredWhenChecked="False" ValidationGroup="vg1"
                                                ErrorMessage="Device IP address is required if it uses static IP" PopupHelpControlID="IPAddressHelpImage"
                                                Display="None"></clearcanvas:ConditionalRequiredFieldValidator>
                                            <clearcanvas:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator1"
                                                runat="server" ControlToValidate="IPAddressTextBox" InvalidInputBackColor="#FAFFB5"
                                                ValidationGroup="vg1" ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
                                                ErrorMessage="The IP address must be of form AAA.BBB.CCC.DDD\nAll components must be in the range from 0 to 255"
                                                Display="None" PopupHelpControlID="IPAddressHelpImage"></clearcanvas:RegularExpressionFieldValidator>
                                        </td>
                                        <td id="Td5" runat="server" align="left" colspan="1" valign="bottom">
                                        </td>
                                        <td id="Td6" runat="server" align="left" colspan="1" valign="bottom">
                                            Partition<br />
                                            <asp:DropDownList ID="ServerPartitionDropDownList" runat="server" Width="150px">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr id="Tr3" runat="server">
                                        <td id="Td7" runat="server">
                                            Port<br />
                                            <asp:TextBox ID="PortTextBox" runat="server"></asp:TextBox><asp:Image ID="PortHelpImage"
                                                runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png" Style="visibility: hidden" />
                                            <clearcanvas:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                ErrorMessage="Device Port must be between 1 and 65535" PopupHelpControlID="PortHelpImage"
                                                Display="None"></clearcanvas:RangeValidator>
                                        </td>
                                        <td id="Td8" runat="server">
                                        </td>
                                        <td id="Td9" runat="server" valign="bottom">
                                            <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="True" Text="Enabled" /></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <cc1:NumericUpDownExtender ID="PortTextBoxNumericUpDownExtender" runat="server" Maximum="65536"
                                Minimum="1" TargetControlID="PortTextBox" Width="100" Enabled="True" RefValues=""
                                ServiceDownMethod="" ServiceDownPath="" ServiceUpMethod="" Tag="" TargetButtonDownID=""
                                TargetButtonUpID="">
                            </cc1:NumericUpDownExtender>
                        </ContentTemplate>
                        <HeaderTemplate>
                            General
                        </HeaderTemplate>
                    </cc1:TabPanel>
                    <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                        <ContentTemplate>
                            <asp:Panel ID="Panel2" runat="server" CssClass="Panel">
                                <table width="100%">
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="AllowStorageCheckBox" runat="server" Text="Storage" ToolTip="Accept or reject C-STORE from this device." /></td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="AllowQueryCheckBox" runat="server" Text="Query" ToolTip="Accept or reject C-FIND from this device" /></td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="AllowRetrieveCheckBox" runat="server" Text="Retrieve" ToolTip="Accept or reject C-MOVE and C-GET from this device" /></td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                        <HeaderTemplate>
                            Features
                        </HeaderTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>
                <table width="100%">
                    <tr>
                        <td align="center">
                            <br />
                            <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click"
                                ValidationGroup="vg1" />
                            &nbsp; &nbsp;&nbsp;
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                    </tr>
                </table>
                <br />
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="36px" Style="z-index: 101;
                    left: 522px; position: absolute; top: 53px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender" runat="server"
            Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass="modalBackground">
        </cc1:ModalPopupExtender>
        &nbsp;
    </ContentTemplate>
</asp:UpdatePanel>
