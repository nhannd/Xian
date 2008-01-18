<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.AddEditPartitionDialog"
    Codebehind="AddEditPartitionDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ccAjax" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Style="display: none"
            Width="400px">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="CSSPopupWindowTitleBar" Width="100%">
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
            <div class="CSSPopupWindowBody">
                <ccAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0"
                    CssClass="CSSDialogTabControl">
                    <ccAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel"
                        CssClass="CSSTabPanel">
                        <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                                <table id="GeneralTabTable" runat="server" width="100%" cellspacing="2">
                                    <tr runat="server" align="left">
                                        <td runat="server">
                                            <asp:Label runat="server" Text="AE Title" CssClass="CSSTextLabel" /><br />
                                            <asp:TextBox ID="AETitleTextBox" runat="server" MaxLength="16" ValidationGroup="vg1"
                                                ToolTip="The DICOM Application Entity Title for the partition."></asp:TextBox>
                                            <clearcanvas:ConditionalRequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                ControlToValidate="AETitleTextBox" Display="None" EnableClientScript="true" ErrorMessage="AE Title is required"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" PopupHelpControlID="AETitleHelpImage"></clearcanvas:ConditionalRequiredFieldValidator>
                                            <clearcanvas:RegularExpressionFieldValidator ID="RegularExpressionFieldValidator2"
                                                runat="server" ControlToValidate="AETitleTextBox" Display="None" ErrorMessage="The AE Title is not valid."
                                                InvalidInputBackColor="#FAFFB5" ValidationExpression="^([^\\]){1,16}$" ValidationGroup="vg1"
                                                PopupHelpControlID="AETitleHelpImage"></clearcanvas:RegularExpressionFieldValidator>
                                        </td>
                                        <td>
                                            <asp:Image ID="AETitleHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden;" />
                                        </td>
                                        <td runat="server" align="left" valign="top">
                                            <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                            <asp:TextBox ID="DescriptionTextBox" runat="server" ToolTip="A textual description of the partition."></asp:TextBox>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr runat="server" align="left">
                                        <td runat="server">
                                            <asp:Label ID="Label2" runat="server" Text="Port" CssClass="CSSTextLabel" /><br />
                                            <asp:TextBox ID="PortTextBox" runat="server"></asp:TextBox>
                                            <asp:Image ID="PortHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden" />
                                            <clearcanvas:RangeValidator ID="PortValidator1" runat="server" ControlToValidate="PortTextBox"
                                                InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="65535"
                                                ErrorMessage="Parition Port must be between 1 and 65535" Display="None" PopupHelpControlID="PortHelpImage"></clearcanvas:RangeValidator>
                                        </td>
                                        <td runat="server">
                                        </td>
                                        <td runat="server">
                                            <asp:Label ID="Label3" runat="server" Text="Folder Name" CssClass="CSSTextLabel" /><br />
                                            <asp:TextBox ID="PartitionFolderTextBox" runat="server" CausesValidation="true" ValidationGroup="vg1"
                                                ToolTip="A unique folder name to store images within for the partition."></asp:TextBox>
                                            <clearcanvas:ConditionalRequiredFieldValidator ID="Conditionalrequiredfieldvalidator1"
                                                runat="server" ControlToValidate="PartitionFolderTextBox" Display="None" EnableClientScript="true"
                                                ErrorMessage="Folder Name is required" InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1"
                                                PopupHelpControlID="FolderHelpImage"></clearcanvas:ConditionalRequiredFieldValidator>
                                        </td>
                                        <td>
                                            <asp:Image ID="FolderHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                Style="visibility: hidden" />
                                        </td>
                                    </tr>
                                    <tr runat="server" align="left">
                                        <td runat="server">
                                            <asp:CheckBox ID="EnabledCheckBox" runat="server" Checked="True" Text="Enabled" ToolTip="Enable or Disable DICOM connections to the partition." />
                                        </td>
                                        <td runat="server" valign="top">
                                        </td>
                                        <td runat="server" valign="top">
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                        <HeaderTemplate>
                            General
                        </HeaderTemplate>
                    </ccAjax:TabPanel>
                    <ccAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                        <ContentTemplate>
                            <asp:Panel ID="Panel2" runat="server" CssClass="CSSDialogTabPanelContent">
                                <table width="100%">
                                    <tr>
                                        <td align="left" style="height: 29px">
                                            <asp:CheckBox ID="AcceptAnyDeviceCheckBox" runat="server" Text="Accept Any Device"
                                                ToolTip="Accept DICOM Associations from any device to this partition." />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="height: 29px">
                                            <asp:CheckBox ID="AutoInsertDeviceCheckBox" runat="server" Text="Auto Insert Devices"
                                                ToolTip="Automatically add devices when they connect to this partition." />
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
                                                PopupHelpControlID="PortHelpImage"/>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </ContentTemplate>
                        <HeaderTemplate>
                            Remote Devices
                        </HeaderTemplate>
                    </ccAjax:TabPanel>
                </ccAjax:TabContainer>
                <br />
                <center>
                    <table cellpadding="0" cellspacing="0" width="80%">
                        <tr align="center">
                            <td>
                                <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click"
                                    ValidationGroup="vg1" />
                            </td>
                            <td>
                                <asp:Button ID="CancelButton" runat="server" Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </center>
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Style="z-index: 101; left: 522px;
                    position: absolute; top: 53px" Width="36px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <ccAjax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" BackgroundCssClass="CSSModalBackground"
            BehaviorID="MyStupidExtender" Enabled="true" PopupControlID="DialogPanel" TargetControlID="DummyPanel">
        </ccAjax:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
&nbsp; 