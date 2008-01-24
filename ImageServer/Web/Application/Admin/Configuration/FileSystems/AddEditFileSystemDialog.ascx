<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.AddFilesystemDialog"
    Codebehind="AddEditFileSystemDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Width="400px"
            Style="display: none">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="CSSPopupWindowTitleBar" Width="100%">
                <table style="width: 100%">
                    <tr>
                        <td valign="middle">
                            <asp:Label ID="TitleLabel" runat="server" Text="Add/Edit FileSystem" Width="100%"
                                EnableViewState="False"></asp:Label></td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="CSSPopupWindowBody" style="vertical-align: top;">
                <asp:Panel ID="Panel2" runat="server" Height="200px" CssClass="CSSDialogTabPanelContent">
                    <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="160px"
                        CssClass="CSSDialogTabControl">
                        <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="CSSTabPanel">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                                    <table id="TABLE1" runat="server" cellspacing="4">
                                        <tr align="left" valign="bottom" runat="server">
                                            <td runat="server">
                                                <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="DescriptionTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" MaxLength="128" ValidationGroup="vg1"></asp:TextBox>
                                                <asp:Image ID="DescriptionHelpImage" runat="server" ImageAlign="bottom" ImageUrl="~/images/icons/HelpSmall.png"
                                                    Style="visibility: hidden" />
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                    runat="server" ControlToValidate="DescriptionTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" ErrorMessage="Description is required!!" Display="None"
                                                    PopupHelpControlID="DescriptionHelpImage" RequiredWhenChecked="False">
                                                </clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td runat="server">
                                                <asp:CheckBox ID="ReadCheckBox" runat="server" OnInit="ReadOnlyCheckBox_Init" Text="Read"
                                                    Checked="True" />
                                            </td>
                                        </tr>
                                        <tr align="left" valign="bottom" runat="server">
                                            <td runat="server">
                                                <asp:Label ID="Label2" runat="server" Text="Path" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="PathTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="256"></asp:TextBox>
                                                <asp:Image ID="PathHelpImage" runat="server" ImageAlign="bottom" ImageUrl="~/images/icons/HelpSmall.png"
                                                    Style="visibility: hidden" />
                                                <clearcanvas:WebServiceValidator runat="server" ID="PathValidator" ControlToValidate="PathTextBox" InputName="Filesystem Path"
                                                    InvalidInputBackColor="#FAFFB5" PopupHelpControlID="PathHelpImage" ServicePath="/Services/ValidationServices.asmx"
                                                    ServiceOperation="ValidateFilesystemPath" ParamsFunction="ValidationFilesystemPathParams"
                                                    Display="None" ValidationGroup="vg1" />
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator2"
                                                    runat="server" ControlToValidate="PathTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" ErrorMessage="Filesystem Path is required!!" Display="None"
                                                    PopupHelpControlID="PathHelpImage" RequiredWhenChecked="False">
                                                </clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td runat="server">
                                                <asp:CheckBox ID="WriteCheckBox" runat="server" Text="Write" Checked="True" />
                                            </td>
                                        </tr>
                                        <tr align="left" runat="server">
                                            <td runat="server">
                                                <asp:Label ID="Label3" runat="server" Text="Filesystem" CssClass="CSSTextLabel" /><br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" Width="220px">
                                                </asp:DropDownList>
                                            </td>
                                            <td runat="server">
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <cc2:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="False"
                                    ValidationGroup="vg1" />
                            </ContentTemplate>
                            <HeaderTemplate>
                                General
                            </HeaderTemplate>
                        </cc1:TabPanel>
                        <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                            <ContentTemplate>
                                <asp:Panel ID="Panel3" runat="server" CssClass="CSSDialogTabPanelContent">
                                    <table id="TABLE2" runat="server" cellspacing="4">
                                        <tr id="Tr1" align="left">
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text="High Watermark" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="HighWatermarkTextBox" runat="server"  BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"/>
                                                <asp:Image ID="HighWatermarkHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                    Style="visibility: hidden" />
                                                <clearcanvas:RangeComparisonValidator ID="HighWatermarkValidator" runat="server" ControlToValidate="HighWatermarkTextBox"
                                                    ControlToCompare="LowWatermarkTextBox" GreaterThan="true" InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="99"
                                                    ErrorMessage="High Watermark must be between 1 and 99 and greater than Low Watermark" Display="None" PopupHelpControlID="HighWatermarkHelpImage" />
                                            </td>
                                        </tr>
                                        <tr id="Tr2">
                                            <td id="Td2">
                                                <asp:Label ID="Label5" runat="server" Text="Low Watermark" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="LowWatermarkTextBox" runat="server" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"/>
                                                <asp:Image ID="LowWatermarkHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                    Style="visibility: hidden" />
                                                <clearcanvas:RangeComparisonValidator ID="LowWatermarkValidator" runat="server" ControlToValidate="LowWatermarkTextBox"
                                                    ControlToCompare="HighWatermarkTextBox" GreaterThan="false" InvalidInputBackColor="#FAFFB5" ValidationGroup="vg1" MinValue="1" MaxValue="99"
                                                    ErrorMessage="Low Watermark must be between 1 and 99 and less than High Watermark" Display="None" PopupHelpControlID="LowWatermarkHelpImage" />
                                            </td>
                                        </tr>
                                        <tr id="Tr3">
                                            <td id="Td3">
                                                <asp:Label ID="Label6" runat="server" Text="Current Percent Full" CssClass="CSSTextLabel" /><br />
                                                <asp:Label ID="PercentFullLabel" runat="server" Text="0.00" Width="68px" Style="padding-right: 5px"
                                                    EnableViewState="False"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            <HeaderTemplate>
                                Watermarks
                            </HeaderTemplate>
                        </cc1:TabPanel>
                    </cc1:TabContainer>
                </asp:Panel>
                <center>
                    <br />
                    <table cellpadding="0" cellspacing="0" width="60%">
                        <tr align="center">
                            <td>
                                <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click"
                                    ValidationGroup="vg1" />
                            </td>
                            <td>
                                <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </center>
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="36px" Style="z-index: 100;
                    left: 786px; position: absolute; top: 32px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender" runat="server"
            Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass="CSSModalBackground">
        </cc1:ModalPopupExtender>
        &nbsp; &nbsp;
    </ContentTemplate>
</asp:UpdatePanel>
