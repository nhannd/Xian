<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ccAjax" %>
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
            <cc2:ValidationSummary ID="ValidationSummary1" runat="server" BackColor="#FFFFC0"
                Height="81px" ShowMessageBox="false" ShowSummary="False" Style="left: 462px;
                position: absolute; top: 77px" ValidationGroup="vg1" Width="159px"></cc2:ValidationSummary>
            <div class="PopupWindowBody" style="vertical-align: top;">
                <ccAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0" Height="150px">
                    <ccAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel">
                        <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server" CssClass="Panel">
                                <table id="GeneralTabTable" runat="server" cellspacing="3" style="text-align: left" width="100%">
                                    <tr runat="server">
                                        <td runat="server" valign="top">
                                            AE Title<br />
                                            <asp:TextBox ID="AETitleTextBox" runat="server" MaxLength="16" ValidationGroup="vg1" ToolTip="The DICOM Application Entity Title for the partition."></asp:TextBox>
                                            <asp:Image ID="AETitleHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden" />
                                            <clearcanvas:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="true" ErrorMessage="AE Title is required"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" PopupHelpControlID="AETitleHelpImage"></clearcanvas:ConditionalRequiredFieldValidator>
                                            <clearcanvas:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator2"
                                                runat="server" ControlToValidate="AETitleTextBox" Display="None" ErrorMessage="The AE Title is not valid."
                                                InvalidInputBackColor="#FAFFB5" ValidationExpression="^([^\\]){1,16}$" ValidationGroup="vg1"
                                                PopupHelpControlID="AETitleHelpImage"></clearcanvas:RegularExpressionFieldValidator>
                                        </td>
                                        <td runat="server" align="left" colspan="1" valign="top">
                                        </td>
                                        <td runat="server" colspan="1" style="color: #000000" valign="top">
                                            Description<br />
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" ToolTip="A textual description of the partition."></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr runat="server">
                                        <td runat="server" style="height: 29px" valign="top">
                                            Port<br />
                                            <asp:TextBox ID="PortTextBox" runat="server"></asp:TextBox>
                                            <asp:Image ID="PortHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png" Style="visibility: hidden" />
                                            <clearcanvas:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                ErrorMessage="Parition Port must be between 1 and 65535" Display="None" PopupHelpControlID="PortHelpImage"></clearcanvas:RangeValidator>
                                        </td>
                                        <td  runat="server" align="left" colspan="1" style="height: 29px" valign="top">
                                        </td>
                                        <td runat="server" align="left" colspan="1" style="height: 29px" valign="top">
                                            Folder Name<br />
                                            <asp:TextBox ID="PartitionFolderTextBox" runat="server" CausesValidation="true" ValidationGroup="vg1" ToolTip="A unique folder name to store images within for the partition."></asp:TextBox>
                                            <asp:Image
                                                ID="FolderHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden" />
                                            <clearcanvas:ConditionalRequiredFieldValidator ID="Conditionalrequiredfieldvalidator1"
                                                runat="server" ControlToValidate="PartitionFolderTextBox" Display="None" EnableClientScript="true"
                                                ErrorMessage="Folder Name is required" InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1"
                                                PopupHelpControlID="FolderHelpImage"></clearcanvas:ConditionalRequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr runat="server">
                                        <td runat="server" style="height: 43px" valign="top">
                                            <asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="True" Text="Enabled" ToolTip="Enable or Disable DICOM connections to the partition."/>
                                        </td>
                                        <td runat="server" style="height: 43px" valign="top">
                                        </td>
                                        <td runat="server" style="height: 43px" valign="top">
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <ccAjax:NumericUpDownExtender ID="PortTextBoxNumericUpDownExtender" runat="server"
                                Maximum="65535" Minimum="1" TargetControlID="PortTextBox" Width="100">
                            </ccAjax:NumericUpDownExtender>
                        </ContentTemplate>
                        <HeaderTemplate>
                            General
                        </HeaderTemplate>
                    </ccAjax:TabPanel>
                    <ccAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                        <ContentTemplate>
                            <asp:Panel ID="Panel2" runat="server" CssClass="Panel">
                                <table width="100%">
                                    <tr>
                                        <td align="left" style="height: 29px">
                                            <asp:CheckBox ID="AcceptAnyDeviceCheckBox" runat="server" Text="Accept Any Device" ToolTip="Accept DICOM Associations from any device to this partition." />
                                        </td>
                                    </tr>
                                    <tr>                             
                                        <td align="left" style="height: 29px">
                                            <asp:CheckBox ID="AutoInsertDeviceCheckBox" runat="server" Text="Auto Insert Devices" ToolTip="Automatically add devices when they connect to this partition." />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" runat="server" style="height: 43px" valign="bottom">
                                            Default Remote Port<br />
                                            <asp:TextBox ID="DefaultRemotePortTextBox" runat="server"></asp:TextBox>
                                            <asp:Image ID="Image1" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden" />
                                            <clearcanvas:RangeValidator ID="DefaultRemotePortRangeValidator" runat="server" ControlToValidate="DefaultRemotePortTextBox"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                ErrorMessage="Parition Default Remote Port must be between 1 and 65535" Display="None"
                                                PopupHelpControlID="PortHelpImage"></clearcanvas:RangeValidator>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <ccAjax:NumericUpDownExtender ID="DefaultRemotePortNumericUpDownExtender" runat="server"
                                Maximum="65535" Minimum="1" TargetControlID="DefaultRemotePortTextBox" Width="100">
                            </ccAjax:NumericUpDownExtender>
                        </ContentTemplate>
                        <HeaderTemplate>
                            Remote Devices
                        </HeaderTemplate>
                    </ccAjax:TabPanel>
                </ccAjax:TabContainer>
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
        <ccAjax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" BackgroundCssClass="modalBackground"
            BehaviorID="MyStupidExtender" Enabled="true" PopupControlID="DialogPanel" TargetControlID="DummyPanel">
        </ccAjax:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
&nbsp; 