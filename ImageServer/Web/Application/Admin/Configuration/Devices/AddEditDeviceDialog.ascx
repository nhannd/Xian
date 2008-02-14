<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.AddEditDeviceDialog"
    Codebehind="AddEditDeviceDialog.ascx.cs" %>
<%@ Register Src="~/Common/InvalidInputIndicator.ascx" TagName="InvalidInputIndicator"
    TagPrefix="CCCommon" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.Validators"
    TagPrefix="CCValidators" %>
    
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Width="442px"
            Style="display: none">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="CSSPopupWindowTitleBar" Width="100%">
                <table style="width: 100%">
                    <tr>
                        <td valign="middle">
                            <asp:Label ID="TitleLabel" runat="server" Text="Add Device" Width="100%" EnableViewState="False"></asp:Label></td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="CSSPopupWindowBody">
                <asp:Panel ID="Panel3" runat="server" Height="100%" CssClass="CSSDialogTabPanelContent">
                    <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="CSSDialogTabControl">
                        <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="CSSTabPanel">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                                    <table id="Table2" runat="server">
                                        <tr id="Tr1" runat="server" align="left">
                                            <td id="Td1" runat="server" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="CSSTextLabel" /><br />
                                                            <asp:TextBox ID="AETitleTextBox" runat="server" ValidationGroup="vg1" MaxLength="16"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <CCCommon:InvalidInputIndicator ID="AETitleHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png"
                                                                Visible="true"></CCCommon:InvalidInputIndicator>
                                                            <CCValidators:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                                ControlToValidate="AETitleTextBox" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                                InvalidInputIndicatorID="AETitleHelp" ErrorMessage="AE Title is required" Display="None"
                                                                RequiredWhenChecked="False">
                                                            </CCValidators:ConditionalRequiredFieldValidator><CCValidators:RegularExpressionFieldValidator
                                                                ID="RegularExpressionFieldValidator2" runat="server" ControlToValidate="AETitleTextBox"
                                                                InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="AETitleHelp"
                                                                ValidationExpression="^([^\\]){1,16}$" ErrorMessage="Invalid AE Title" Display="None"></CCValidators:RegularExpressionFieldValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td id="Td2" runat="server" align="left" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:Label ID="Label2" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                                            <asp:TextBox ID="DescriptionTextBox" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr id="Tr2" runat="server" align="left" valign="bottom">
                                            <td id="Td4" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:Label ID="Label3" runat="server" Text="IP Address" CssClass="CSSTextLabel" />
                                                            <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" /><br />
                                                            <asp:TextBox ID="IPAddressTextBox" runat="server" ValidationGroup="vg1">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td align="left">
                                                            <CCCommon:InvalidInputIndicator ID="IPAddressHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                            </CCCommon:InvalidInputIndicator>
                                                            <CCValidators:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                                runat="server" ControlToValidate="IPAddressTextBox" InvalidInputColor="#FAFFB5"
                                                                ConditionalCheckBoxID="DHCPCheckBox" RequiredWhenChecked="False" ValidationGroup="vg1"
                                                                ErrorMessage="Device IP address is required if it uses static IP" InvalidInputIndicatorID="IPAddressHelp"
                                                                Display="None"></CCValidators:ConditionalRequiredFieldValidator>
                                                            <CCValidators:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator1"
                                                                runat="server" ControlToValidate="IPAddressTextBox" InvalidInputColor="#FAFFB5"
                                                                IgnoreEmptyValue="true" ValidationGroup="vg1" ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
                                                                ErrorMessage="IP address is malformed" Display="None" InvalidInputIndicatorID="IPAddressHelp"></CCValidators:RegularExpressionFieldValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td id="Td6" runat="server" align="left" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:Label ID="Label4" runat="server" Text="Partition" CssClass="CSSTextLabel" /><br />
                                                            <asp:DropDownList ID="ServerPartitionDropDownList" runat="server" Width="100%">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr id="Tr3" runat="server" align="left" valign="bottom">
                                            <td id="Td7" runat="server" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:Label ID="Label5" runat="server" Text="Port" CssClass="CSSTextLabel" /><br />
                                                            <asp:TextBox ID="PortTextBox" runat="server" />
                                                        </td>
                                                        <td>
                                                            <CCCommon:InvalidInputIndicator ID="PortHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png"
                                                                Visible="true"></CCCommon:InvalidInputIndicator>
                                                            <CCValidators:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                                InvalidInputColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                                ErrorMessage="Device Port must be between 1 and 65535" InvalidInputIndicatorID="PortHelp"
                                                                Display="None"></CCValidators:RangeValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td id="Td9" runat="server" valign="bottom">
                                                <table width="100%">
                                                    <tr align="left">
                                                        <td width="100%">
                                                            <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="True" Text="Enabled" />
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            <HeaderTemplate>
                                General
                            </HeaderTemplate>
                        </ajaxToolkit:TabPanel>
                        <ajaxToolkit:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2" Height="200px">
                            <ContentTemplate>
                                <asp:Panel ID="Panel2" runat="server" CssClass="CSSDialogTabPanelContent">
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
                        </ajaxToolkit:TabPanel>
                    </ajaxToolkit:TabContainer>
                </asp:Panel>
                <br />
                <center>
                    <table width="80%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click"
                                    ValidationGroup="vg1" />
                            </td>
                            <td align="center">
                                <asp:Button ID="CancelButton" runat="server" Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </center>
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="36px" Style="z-index: 101;
                    left: 522px; position: absolute; top: 53px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender" runat="server"
            Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass="CSSModalBackground">
        </ajaxToolkit:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
