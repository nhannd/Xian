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
                                        <tr align="left">
                                            <td>
                                                <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="DescriptionTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" MaxLength="128" ValidationGroup="vg1"></asp:TextBox><asp:Image
                                                        ID="DescriptionHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                        Style="visibility: hidden" />
                                            </td>
                                            <td>
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                    runat="server" ControlToValidate="DescriptionTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" EnableClientScript="true" ErrorMessage="Description is required!!"
                                                    Display="None" PopupHelpControlID="DescriptionHelpImage"></clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="ReadCheckBox" runat="server" OnInit="ReadOnlyCheckBox_Init" Text="Read"
                                                    Checked="True" />
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td>
                                                <asp:Label ID="Label2" runat="server" Text="Path" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="PathTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="256"></asp:TextBox><asp:Image
                                                        ID="PathHelpImage" runat="server" ImageAlign="Top" ImageUrl="~/images/icons/HelpSmall.png"
                                                        Style="visibility: hidden" />
                                            </td>
                                            <td>
                                                <clearcanvas:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator2"
                                                    runat="server" ControlToValidate="PathTextBox" InvalidInputBackColor="#FAFFB5"
                                                    ValidationGroup="vg1" EnableClientScript="true" ErrorMessage="Filesystem Path is required!!"
                                                    Display="None" PopupHelpControlID="PathHelpImage">
                                                </clearcanvas:ConditionalRequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="WriteCheckBox" runat="server" Text="Write" Checked="True" />
                                            </td>
                                        </tr>
                                        <tr align="left">
                                            <td>
                                                <asp:Label ID="Label3" runat="server" Text="Filesystem" CssClass="CSSTextLabel" /><br />
                                                <asp:DropDownList ID="TiersDropDownList" runat="server" Width="220px">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
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
                                <asp:Panel ID="Panel3" runat="server" CssClass="CSSDialogTabPanelContent">
                                    <table id="TABLE2" runat="server" cellspacing="4">
                                        <tr id="Tr1" align="left">
                                            <td>
                                                <asp:Label ID="Label4" runat="server" Text="High Watermark" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="HighWatermarkTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr id="Tr2">
                                            <td id="Td2">
                                                <asp:Label ID="Label5" runat="server" Text="Low Watermark" CssClass="CSSTextLabel" /><br />
                                                <asp:TextBox ID="LowWatermarkTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue"
                                                    BorderWidth="1px" ValidationGroup="vg1" MaxLength="7"></asp:TextBox>
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
