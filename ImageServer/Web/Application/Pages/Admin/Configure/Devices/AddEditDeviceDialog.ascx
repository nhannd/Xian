<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.AddEditDeviceDialog"
    CodeBehind="AddEditDeviceDialog.ascx.cs" %>
<%@ Register Src="ThrottleSettingsTab.ascx" TagName="ThrottleSettingsTab" TagPrefix="localAsp" %>
<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="450px">
    <ContentTemplate>
        <ccAsp:JQuery runat="server" />

        <script type="text/javascript">

            function AllowStorage_Changed() {
                if ($("#<%=AllowStorageCheckBox.ClientID %>").attr('checked') == false) {
                    $("#<%=AcceptKOPR.ClientID %>").attr('disabled', 'true');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#bbbbbb");
                } else {
                    $("#<%=AcceptKOPR.ClientID %>").removeAttr('disabled');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#16425D");
                }
            }

            function AcceptKOPR_Changed() {
                if ($("#<%=AcceptKOPR.ClientID %>").attr('checked') == true) {
                    $("#<%=AllowStorageCheckBox.ClientID %>").attr('checked', true);
                }
            }

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SetupCheckBoxes);            

            function SetupCheckBoxes() {
                if ($("#<%=AllowStorageCheckBox.ClientID %>").attr('checked') == false) {
                    $("#<%=AcceptKOPR.ClientID %>").attr('disabled', 'true');
                    $("#<%=AcceptKeyObjectStatesLabel.ClientID %>").css("color", "#bbbbbb");
                }
                if ($("#<%=AcceptKOPR.ClientID %>").attr('checked') == true)
                    $("#<%=AllowStorageCheckBox.ClientID %>").attr('checked', true);
            });       


        </script>

        <asp:ValidationSummary ID="EditDeviceValidationSummary" ShowMessageBox="false" ShowSummary="true"
            DisplayMode="SingleParagraph" EnableClientScript="true" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
            CssClass="DialogValidationErrorMessage" />
        <aspAjax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
            <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="DialogTabControl">
                <ContentTemplate>
                    <table id="Table2" runat="server">
                        <tr id="Tr1" runat="server" align="left">
                            <td id="Td1" runat="server" valign="bottom">
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="AETitleTextBox" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
                                                MaxLength="16" CssClass="DialogTextBox"></asp:TextBox>
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="AETitleHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td2" runat="server" align="left" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text="Description" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="DialogTextBox"></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr id="Tr2" runat="server" align="left" valign="bottom">
                            <td id="Td4" valign="bottom">
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label3" runat="server" Text="IP Address" CssClass="DialogTextBoxLabel" />
                                            <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" CssClass="DialogCheckBox" /><br />
                                            <asp:TextBox ID="IPAddressTextBox" runat="server" ValidationGroup="AddEditDeviceValidationGroup"
                                                CssClass="DialogTextBox">
                                            </asp:TextBox>
                                        </td>
                                        <td align="left" valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="IPAddressHelp" runat="server" SkinID="InvalidInputIndicator">
                                            </ccAsp:InvalidInputIndicator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td6" runat="server" align="left" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label6" runat="server" Text="Device Type" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:DropDownList ID="DeviceTypeDropDownList" runat="server" Width="100%" CssClass="DialogDropDownList">
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
                                <table>
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text="Port" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="PortTextBox" runat="server" CssClass="DialogTextBox" />
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PortHelp" runat="server" SkinID="InvalidInputIndicator">
                                            </ccAsp:InvalidInputIndicator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td9" runat="server" valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text="Partition" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:DropDownList ID="ServerPartitionDropDownList" runat="server" Width="100%" CssClass="DialogDropDownList">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td valign="bottom">
                                <table width="100%">
                                    <tr align="left">
                                        <td>
                                            <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="True" Text="Enabled" CssClass="DialogCheckBox" />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <ccValidator:RegularExpressionFieldValidator ID="AETitleTextBoxValidator" runat="server"
                        ControlToValidate="AETitleTextBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                        ValidationGroup="AddEditDeviceValidationGroup" InvalidInputIndicatorID="AETitleHelp"
                        ValidationExpression="^([^\\]){1,16}$" Text="Invalid AE Title" Display="None"></ccValidator:RegularExpressionFieldValidator>
                    <ccValidator:DeviceValidator ID="DeviceValidator" runat="server"
                        ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="false"
                        Text="The AE Title is not valid." InvalidInputCSS="DialogTextBoxInvalidInput" 
                        ValidationGroup="AddEditDeviceValidationGroup" InvalidInputIndicatorID="AETitleHelp"></ccValidator:DeviceValidator>
                    <ccValidator:ConditionalRequiredFieldValidator ID="IPAddressValidator" runat="server"
                        ControlToValidate="IPAddressTextBox" ValidateWhenUnchecked="true" ConditionalCheckBoxID="DHCPCheckBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                        ValidationGroup="AddEditDeviceValidationGroup" Text="Device IP address is required if it uses static IP"
                        InvalidInputIndicatorID="IPAddressHelp" Display="None"></ccValidator:ConditionalRequiredFieldValidator>
                    <ccValidator:RangeValidator ID="PortValidator" runat="server" ControlToValidate="PortTextBox"
                        ValidationGroup="AddEditDeviceValidationGroup" MinValue="1" MaxValue="65535"
                        InvalidInputCSS="DialogTextBoxInvalidInput" Text="Device Port must be between 1 and 65535"
                        InvalidInputIndicatorID="PortHelp" Display="None"></ccValidator:RangeValidator>
                </ContentTemplate>
                <HeaderTemplate>
                    General</HeaderTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2" Height="200px">
                <ContentTemplate>
                    <table width="100%">
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowStorageCheckBox" runat="server" Text="Storage" ToolTip="Accept or reject C-STORE from this device"
                                    CssClass="DialogCheckBox" />
                                <div style="padding-left: 18px; padding-top: 3px; padding-bottom: 2px;">
                                    <asp:CheckBox ID="AcceptKOPR" runat="server" Checked="False" Text="" CssClass="DialogCheckBox" /><asp:Label
                                        ID="AcceptKeyObjectStatesLabel" runat="server" Text="Only Accept Key Objects and Presentation States"
                                        CssClass="DialogCheckBox" />
                                </div>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowAutoRouteCheckBox" runat="server" Text="Auto Route" ToolTip="Allow auto-routing to this device"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowQueryCheckBox" runat="server" Text="Query" ToolTip="Accept or reject C-FIND from this device"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:CheckBox ID="AllowRetrieveCheckBox" runat="server" Text="Retrieve" ToolTip="Accept or reject C-MOVE and C-GET from this device"
                                    CssClass="DialogCheckBox" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <HeaderTemplate>
                    Features</HeaderTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel ID="TabPanel3" runat="server" HeaderText="TabPanel2" Height="200px">
                <ContentTemplate>
                    <localAsp:ThrottleSettingsTab runat="server" ID="ThrottleSettingsTab" />
                </ContentTemplate>
                <HeaderTemplate>
                    Throttle</HeaderTemplate>
            </aspAjax:TabPanel>
        </aspAjax:TabContainer>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" OnClick="OKButton_Click"
                            ValidationGroup="AddEditDeviceValidationGroup" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
