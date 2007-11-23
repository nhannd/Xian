<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.AddFilesystemDialog"
    Codebehind="AddEditFileSystemDialog.ascx.cs" %>
<%@ Register Assembly="Validators" Namespace="Sample.Web.UI.Compatibility" TagPrefix="cc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls"
    TagPrefix="clearcanvas" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="PopupWindow" Width="400px" Style="display: none">
            <asp:Panel ID="TitleBarPanel" runat="server" CssClass="PopupWindowTitleBar" Width="100%">
                <table style="width: 100%">
                    <tr>
                        <td valign="middle">
                            <asp:Label ID="TitleLabel" runat="server" Text="Add/Edit FileSystem" Width="100%"
                                EnableViewState="False"></asp:Label></td>
                    </tr>
                </table>
            </asp:Panel>
            <div class="PopupWindowBody" style="vertical-align: top;">
                <asp:Panel ID="Panel2" runat="server" Height="200px" CssClass="Panel">
                    <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="160px">
                        <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
                            <ContentTemplate>
                                <asp:Panel ID="Panel1" runat="server" CssClass="Panel">
                                    <table id="TABLE1" runat="server" cellspacing="4" style="text-align: left" width="90%">
                                        <tr runat="server">
                                            <td runat="server" valign="bottom">
                                                Description<br />
                                                <asp:TextBox ID="DescriptionTextBox" runat="server" Width="250px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" MaxLength="128" ValidationGroup="vg1"></asp:TextBox>
                                            </td>
                                            <td colspan="1" runat="server" valign="bottom">
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                    runat="server" ControlToValidate="DescriptionTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" EnableClientScript="true" ErrorMessage="Description is required!!"
                                                    Display="None"></clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td runat="server" colspan="1" style="width: 53px; color: #000000" valign="bottom">
                                                <asp:CheckBox ID="ReadCheckBox" runat="server" OnInit="ReadOnlyCheckBox_Init" Text="Read"
                                                    Checked="True" />
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td style="height: 29px" runat="server" valign="bottom">
                                                Path<br />
                                                <asp:TextBox ID="PathTextBox" runat="server" Width="250px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="256"></asp:TextBox>
                                            </td>
                                            <td colspan="1" style="height: 29px" align="left" runat="server" valign="bottom">
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator2"
                                                    runat="server" ControlToValidate="PathTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" EnableClientScript="true" ErrorMessage="Filesystem Path is required!!"
                                                    Display="None"></clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td runat="server" align="left" colspan="1" style="width: 53px; height: 29px" valign="bottom">
                                                <asp:CheckBox ID="WriteCheckBox" runat="server" Text="Write" Checked="True" />
                                            </td>
                                        </tr>
                                        <tr runat="server">
                                            <td style="height: 43px" runat="server">
                                                Filesystem Tier<br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" Width="250px">
                                                </asp:DropDownList>
                                            </td>
                                            <td runat="server" style="height: 43px">
                                            </td>
                                            <td runat="server" style="height: 43px">
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <cc2:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="false"
                                    ShowSummary="False" ValidationGroup="vg1" />
                            </ContentTemplate>
                            <HeaderTemplate>
                                General
                            </HeaderTemplate>
                        </cc1:TabPanel>
                        <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                            <ContentTemplate>
                                <asp:Panel ID="Panel3" runat="server" CssClass="Panel">
                                    <table id="TABLE2" runat="server" cellspacing="4" style="text-align: left" width="90%">
                                        <tr id="Tr1" runat="server">
                                            <td id="Td1" style="height: 29px" runat="server" valign="bottom">
                                                High Watermark<br />
                                                <asp:TextBox ID="HighWatermarkTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="Tr2" runat="server">
                                            <td id="Td2" style="height: 29px" runat="server" valign="bottom">
                                                Low Watermark<br />
                                                <asp:TextBox ID="LowWatermarkTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="Tr3" runat="server">
                                            <td id="Td3" style="height: 29px" runat="server" valign="bottom">
                                                Current Percent Full<br />
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
                    </cc1:TabContainer></asp:Panel>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="OKButton" runat="server" Text="Add" Width="77px" OnClick="OKButton_Click"
                                ValidationGroup="vg1" />
                            &nbsp;&nbsp;
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /></td>
                    </tr>
                </table>
                <br />
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="36px" Style="z-index: 100;
                    left: 786px; position: absolute; top: 32px">
                </asp:Panel>
            </div>
        </asp:Panel>
        <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="MyStupidExtender" runat="server"
            Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" BackgroundCssClass="modalBackground">
        </cc1:ModalPopupExtender>
        &nbsp; &nbsp;
    </ContentTemplate>
</asp:UpdatePanel>
