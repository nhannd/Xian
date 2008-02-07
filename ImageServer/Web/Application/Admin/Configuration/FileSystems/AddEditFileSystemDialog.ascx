<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.AddFilesystemDialog"
    Codebehind="AddEditFileSystemDialog.ascx.cs" %>
<%@ Register Src="~/Common/InvalidInputIndicator.ascx" TagName="InvalidInputIndicator"
    TagPrefix="uc1" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/Services/FilesystemInfoService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Width="442px"
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
                <asp:Panel ID="Panel2" runat="server" Height="100%" CssClass="CSSDialogTabPanelContent">
                    <ajax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="CSSDialogTabControl">
                        <ajax:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="CSSTabPanel">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                                    <table id="TABLE1" runat="server" cellspacing="4" width="100%">
                                        <tr align="left" valign="bottom" runat="server">
                                            <td runat="server">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                                            <asp:TextBox ID="DescriptionTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                                BorderWidth="1px" MaxLength="128" ValidationGroup="vg1"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <uc1:InvalidInputIndicator ID="InvalidDescriptionHint" runat="server" ImageUrl="~/images/icons/HelpSmall.png"
                                                                Visible="true"></uc1:InvalidInputIndicator>
                                                            <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                                runat="server" ControlToValidate="DescriptionTextBox" InvalidInputColor="#FAFFB5"
                                                                ValidationGroup="vg1" ErrorMessage="Description is required!!" Display="None"
                                                                InvalidInputIndicatorID="InvalidDescriptionHint" RequiredWhenChecked="False">
                                                            </clearcanvas:ConditionalRequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td runat="server">
                                                <table width="100px">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="ReadCheckBox" runat="server" OnInit="ReadOnlyCheckBox_Init" Text="Read"
                                                                Checked="True" TextAlign="Right" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr valign="bottom">
                                            <td runat="server">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label2" runat="server" Text="Path" CssClass="CSSTextLabel" /><br />
                                                            <asp:TextBox ID="PathTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                                BorderWidth="1px" ValidationGroup="vg1" MaxLength="256"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <uc1:InvalidInputIndicator ID="InvalidPathHint" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                            </uc1:InvalidInputIndicator>
                                                            <clearcanvas:FilesystemPathValidator runat="server" ID="PathValidator" ControlToValidate="PathTextBox"
                                                                InputName="Filesystem Path" InvalidInputColor="#FAFFB5" InvalidInputIndicatorID="InvalidPathHint"
                                                                ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateFilesystemPath"
                                                                ParamsFunction="ValidationFilesystemPathParams" Display="None" ValidationGroup="vg1" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td runat="server">
                                                <table width="100px">
                                                    <tr>
                                                        <td>
                                                            <asp:CheckBox ID="WriteCheckBox" runat="server" Text="Write" Checked="True" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr valign="bottom">
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="Label3" runat="server" Text="Filesystem" CssClass="CSSTextLabel" /><br />
                                                            <asp:DropDownList ID="TiersDropDownList" runat="server" Width="220px">
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            <HeaderTemplate>
                                General
                            </HeaderTemplate>
                        </ajax:TabPanel>
                        <ajax:TabPanel ID="TabPanel2" runat="server" HeaderText="Watermarks Tab" OnClientClick="LoadFilesystemInfo">
                            <ContentTemplate>
                                <asp:Panel ID="Panel3" runat="server" CssClass="CSSDialogTabPanelContent" Width="100%">
                                    <table id="TABLE2" runat="server" cellspacing="4">
                                        <!-- total size -->
                                        <tr id="Tr4" align="left" valign="bottom">
                                            <td>
                                                <asp:Panel runat="server" ID="TotalSizePanel">
                                                    <table>
                                                        <tr>
                                                            <td width="120px" align="left" valign="bottom">
                                                                <asp:Label ID="Label7" runat="server" Text="Total Size" CssClass="CSSTextLabel" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="TotalSizeIndicator" runat="server" Text="??? KB" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <!-- available size -->
                                        <tr id="Tr3" align="left" valign="bottom">
                                            <td>
                                                <asp:Panel runat="server" ID="AvailableSizePanel">
                                                    <table>
                                                        <tr>
                                                            <td width="120px" align="left" valign="bottom">
                                                                <asp:Label ID="Label8" runat="server" Text="Available" CssClass="CSSTextLabel" />
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="AvailableSizeIndicator" runat="server" Text="??? KB" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <!-- highwatermark -->
                                        <tr id="Tr5" align="left" valign="bottom">
                                            <td>
                                                <asp:Panel runat="server" ID="HighWatermarkPanel">
                                                    <table>
                                                        <tr>
                                                            <td width="120px" align="left" valign="bottom">
                                                                <asp:Label ID="Label4" runat="server" Text="High Watermark" CssClass="CSSTextLabel" /><br />
                                                                <asp:TextBox ID="HighWatermarkTextBox" runat="server" BorderColor="LightSteelBlue"
                                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="8" Width="100px" />%
                                                            </td>
                                                            <td align="left" valign="bottom">
                                                                <asp:TextBox runat="server" ID="HighWatermarkSize" BorderColor="LightSteelBlue" BorderWidth="1px"
                                                                    Text="???.??? GB" Enabled="false" Width="80px" Style="text-align: right" />
                                                            </td>
                                                            <td align="left" valign="bottom">
                                                                <uc1:InvalidInputIndicator ID="HighWatermarkHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                                </uc1:InvalidInputIndicator>
                                                                <clearcanvas:RangeComparisonValidator ID="HighWatermarkValidator" runat="server"
                                                                    ControlToValidate="HighWatermarkTextBox" ControlToCompare="LowWatermarkTextBox"
                                                                    GreaterThan="true" InvalidInputColor="#FAFFB5" ValidationGroup="vg1" MinValue="1"
                                                                    MaxValue="99" InputName="High watermark" CompareToInputName="Low watermark" Display="None"
                                                                    InvalidInputIndicatorID="HighWatermarkHelp" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <!-- low w -->
                                        <tr id="Tr1" align="left" valign="bottom">
                                            <td>
                                                <asp:Panel runat="server" ID="Panel4">
                                                    <table>
                                                        <tr>
                                                            <td width="120px" align="left" valign="bottom">
                                                                <asp:Label ID="Label5" runat="server" Text="Low Watermark" CssClass="CSSTextLabel" /><br />
                                                                <asp:TextBox ID="LowWatermarkTextBox" runat="server" BorderColor="LightSteelBlue"
                                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="8" Width="100px" />%
                                                            </td>
                                                            <td align="left" valign="bottom">
                                                                <asp:TextBox runat="server" ID="LowWaterMarkSize" BorderColor="LightSteelBlue" BorderWidth="1px"
                                                                    Text="???.??? GB" Enabled="false" Width="80px" Style="text-align: right" />
                                                            </td>
                                                            <td align="left" valign="bottom">
                                                                <uc1:InvalidInputIndicator ID="LowWatermarkHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                                </uc1:InvalidInputIndicator>
                                                                <clearcanvas:RangeComparisonValidator ID="LowWatermarkValidator" EnableClientScript="true"
                                                                    runat="server" ControlToValidate="LowWatermarkTextBox" ControlToCompare="HighWatermarkTextBox"
                                                                    GreaterThan="false" InvalidInputColor="#FAFFB5" ValidationGroup="vg1" MinValue="1"
                                                                    MaxValue="99" InputName="Low watermark" CompareToInputName="High watermark" Display="None"
                                                                    InvalidInputIndicatorID="LowWatermarkHelp" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </ContentTemplate>
                            <HeaderTemplate>
                                Watermarks
                            </HeaderTemplate>
                        </ajax:TabPanel>
                    </ajax:TabContainer>
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
                <asp:HiddenField ID="TotalSize" runat="server" />
                <asp:HiddenField ID="AvailableSize" runat="server" />
            </div>
        </asp:Panel>
        <ajax:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender" runat="server"
            Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass="CSSModalBackground">
        </ajax:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
