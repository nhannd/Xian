<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.FileSystems.AddFilesystemDialog"
    Codebehind="AddEditFileSystemDialog.ascx.cs" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/Services/FilesystemInfoService.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="450px">
    <ContentTemplate>
        <asp:Panel ID="Panel2" runat="server">
            <aspAjax:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="CSSDialogTabControl">
                <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1" CssClass="CSSTabPanel">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                            <table id="TABLE1" runat="server" cellspacing="4" width="100%">
                                <tr id="Tr1" align="left" valign="bottom" runat="server">
                                    <td id="Td1" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label1" runat="server" Text="Description" CssClass="CSSTextLabel" /><br />
                                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                        BorderWidth="1px" MaxLength="128" ValidationGroup="vg1"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <ccAsp:InvalidInputIndicator ID="InvalidDescriptionHint" runat="server" ImageUrl="~/images/icons/HelpSmall.png"
                                                        Visible="true"></ccAsp:InvalidInputIndicator>
                                                    <ccValidator:ConditionalRequiredFieldValidator ID="ConditionalRequiredFieldValidator1"
                                                        runat="server" ControlToValidate="DescriptionTextBox" InvalidInputColor="#FAFFB5"
                                                        ValidationGroup="vg1" ErrorMessage="Description is required!!" Display="None"
                                                        InvalidInputIndicatorID="InvalidDescriptionHint" RequiredWhenChecked="False">
                                                    </ccValidator:ConditionalRequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td2" runat="server">
                                        <table width="100px">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="ReadCheckBox" runat="server"  Text="Read"
                                                        Checked="True" TextAlign="Right" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr valign="bottom">
                                    <td id="Td3" runat="server">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="Path" CssClass="CSSTextLabel" /><br />
                                                    <asp:TextBox ID="PathTextBox" runat="server" Width="220px" BorderColor="LightSteelBlue"
                                                        BorderWidth="1px" ValidationGroup="vg1" MaxLength="256"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <ccAsp:InvalidInputIndicator ID="InvalidPathHint" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                    </ccAsp:InvalidInputIndicator>
                                                    <ccValidator:FilesystemPathValidator runat="server" ID="PathValidator" ControlToValidate="PathTextBox"
                                                        InputName="Filesystem Path" InvalidInputColor="#FAFFB5" InvalidInputIndicatorID="InvalidPathHint"
                                                        ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateFilesystemPath"
                                                        ParamsFunction="ValidationFilesystemPathParams" Display="None" ValidationGroup="vg1" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="Td4" runat="server">
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
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="Watermarks Tab" OnClientClick="LoadFilesystemInfo">
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
                                                        <asp:Label ID="Label8" runat="server" Text="Used" CssClass="CSSTextLabel" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="UsedSizeIndicator" runat="server" Text="??? KB" />
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
                                                        <ccAsp:InvalidInputIndicator ID="HighWatermarkHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                        </ccAsp:InvalidInputIndicator>
                                                        <ccValidator:RangeComparisonValidator ID="HighWatermarkValidator" runat="server"
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
                                <tr id="Tr2" align="left" valign="bottom">
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
                                                        <ccAsp:InvalidInputIndicator ID="LowWatermarkHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                        </ccAsp:InvalidInputIndicator>
                                                        <ccValidator:RangeComparisonValidator ID="LowWatermarkValidator" EnableClientScript="true"
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
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
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
        <asp:HiddenField ID="TotalSize" runat="server" />
        <asp:HiddenField ID="AvailableSize" runat="server" />
    </ContentTemplate>
</ccAsp:ModalDialog>
