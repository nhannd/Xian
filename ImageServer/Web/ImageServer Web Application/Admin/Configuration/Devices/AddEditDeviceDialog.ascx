<%@ Control Language="C#" AutoEventWireup="true" Inherits="ImageServerWebApplication.Admin.Configuration.Devices.AddEditDeviceDialog"
    Codebehind="AddEditDeviceDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.WebControls" Namespace="ClearCanvas.ImageServer.Web.WebControls"
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
            <cc2:ValidationSummary ID="ValidationSummary1" runat="server" Style="left: 462px;
                position: absolute; top: 77px" BackColor="#FFFFC0" Height="81px" ValidationGroup="vg1"
                Width="159px" ShowMessageBox="true" ShowSummary="False" />
            <div class="PopupWindowBody" style="vertical-align: top; text-align: center">
                <asp:Panel ID="Panel1" runat="server" CssClass="Panel">
                    <table id="Table2" runat="server" cellspacing="4" style="text-align: left" width="90%">
                        <tr id="Tr1" runat="server">
                            <td id="Td1" runat="server" valign="bottom">
                                AE Title<br />
                                <asp:TextBox ID="AETitleTextBox" runat="server" ValidationGroup="vg1" MaxLength="16" ></asp:TextBox>
                                <clearcanvas:ConditionalRequiredFieldValidator 
                                    ID="RequiredFieldValidator2" runat="server" ControlToValidate="AETitleTextBox"
                                    InvalidInputBackColor="#FAFFB5"
                                    ValidationGroup="vg1" EnableClientScript="true"
                                    ErrorMessage="AE Title is required!!" Display="None">
                                 </clearcanvas:ConditionalRequiredFieldValidator>
                                 <clearcanvas:RegularExpressionFieldValidator 
                                    ID="RegularExpressionFieldValidator2" runat="server"
                                    ControlToValidate="AETitleTextBox"
                                    InvalidInputBackColor="#FAFFB5"
                                    ValidationGroup="vg1" 
                                    ValidationExpression="^(\w|[ ][^\\]){1,16}$"
                                    ErrorMessage="The AE Title is not valid." Display="None">
                                    </clearcanvas:RegularExpressionFieldValidator>
                                 </td>
                            <td id="Td2" runat="server" colspan="1" valign="bottom" align="left">
                                </td>
                            <td id="Td3" runat="server" colspan="1" style="color: #000000" valign="bottom">
                                Description<br />
                                <asp:TextBox ID="DescriptionTextBox" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr id="Tr2" runat="server">
                            <td id="Td4" runat="server" style="height: 29px" valign="bottom">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td>
                                            IP Address
                                        </td>
                                        <td align="center">
                                            <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" Width="68px" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                <asp:TextBox ID="IPAddressTextBox" runat="server" CausesValidation="true" ValidationGroup="vg1"></asp:TextBox></td>
                                    </tr>
                                </table>
                                <clearcanvas:ConditionalRequiredFieldValidator 
                                    ID="ConditionalRequiredFieldValidator1" runat="server"
                                    ControlToValidate="IPAddressTextBox"
                                    InvalidInputBackColor="#FAFFB5"
                                    ConditionalCheckBoxID="DHCPCheckBox" 
                                    RequiredWhenChecked="false"
                                    ValidationGroup="vg1" 
                                    ErrorMessage="Device IP address is required if it uses static IP" 
                                    Display="None">
                                    </clearcanvas:ConditionalRequiredFieldValidator>
                                    
                                    
                                <clearcanvas:RegularExpressionFieldValidator 
                                    ID="RegularExpressionFieldValidator1" runat="server"
                                    ControlToValidate="IPAddressTextBox"
                                    InvalidInputBackColor="#FAFFB5"
                                    ValidationGroup="vg1" 
                                    ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
                                    ErrorMessage="The IP address is not valid." Display="None">
                                    </clearcanvas:RegularExpressionFieldValidator>
                                    
                                
                                </td>
                            <td id="Td5" runat="server" align="left" colspan="1" style="height: 29px" valign="bottom">
                            </td>
                            <td id="Td6" runat="server" align="left" colspan="1" style="height: 29px" valign="bottom">
                                Partition<br />
                                <asp:DropDownList ID="ServerPartitionDropDownList" runat="server">
                                </asp:DropDownList></td>
                        </tr>
                        <tr id="Tr3" runat="server">
                            <td id="Td7" runat="server" style="height: 43px">
                                Port<br />
                                <asp:TextBox ID="PortTextBox" runat="server"></asp:TextBox></td>
                            <td id="Td8" runat="server" style="height: 43px">
                            </td>
                            <td id="Td9" runat="server" style="height: 43px" valign="bottom">
                                <asp:CheckBox ID="ActiveCheckBox" runat="server" Checked="True" Text="Active" /></td>
                        </tr>
                    </table>
                </asp:Panel>
                <table width="80%">
                    <tr>
                        <td>
                        </td>
                        <td style="width: 102px">
                        </td>
                        <td style="width: 89px">
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="center" style="width: 102px">
                            <asp:Button ID="OKButton" runat="server"  Text="Add" Width="77px" 
                            OnClick="OKButton_Click"   ValidationGroup="vg1"/></td>
                        <td align="center" style="width: 89px">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td style="width: 102px">
                        </td>
                        <td style="width: 89px">
                        </td>
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
        <cc1:NumericUpDownExtender ID="PortTextBoxNumericUpDownExtender" runat="server" Maximum="65536"
            Minimum="0" TargetControlID="PortTextBox" Width="100">
        </cc1:NumericUpDownExtender>
        
    </ContentTemplate>
</asp:UpdatePanel>
