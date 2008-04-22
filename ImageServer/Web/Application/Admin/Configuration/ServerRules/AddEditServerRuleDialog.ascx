<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AddEditServerRuleDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.AddEditServerRuleDialog" %>

<%@ Register Src="~/Common/InvalidInputIndicator.ascx" TagName="InvalidInputIndicator"
    TagPrefix="CCCommon" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.Validators"
    TagPrefix="CCValidators" %>
<%@ Register Src="~/Common/ModalDialog.ascx" TagName="ModalDialog" TagPrefix="clearcanvas" %>
<asp:ScriptManagerProxy runat="server">
    <Services>
        <asp:ServiceReference Path="ServerRuleSamples.asmx" />
    </Services>
</asp:ScriptManagerProxy>

<clearcanvas:ModalDialog ID="ModalDialog" runat="server" BackgroundCSS="CSSDefaultPopupDialogBackground" Width="700px">
    <ContentTemplate>
        <asp:Panel runat="server">
            <aspAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0"
                CssClass="CSSDialogTabControl">
                <aspAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel"
                    TabIndex="0" CssClass="CSSTabPanel">
                    <ContentTemplate>
                        <asp:Panel ID="Panel1" runat="server" CssClass="CSSDialogTabPanelContent">
                            <table id="Table1" runat="server" width="100%">
                                <tr>
                                    <td colspan="2">
                                        <table width="100%">
                                            <tr>
                                                <td width="100%">
                                                    <asp:Label ID="RuleNameLabel" runat="server" Text="Name" CssClass="CSSTextLabel"></asp:Label><br />
                                                    <asp:TextBox ID="RuleNameTextBox" runat="server" Width="100%" ValidationGroup="vg1"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <CCCommon:InvalidInputIndicator ID="RuleNameHelp" runat="server" ImageUrl="~/images/icons/HelpSmall.png">
                                                    </CCCommon:InvalidInputIndicator>
                                                    <CCValidators:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
                                                        ControlToValidate="RuleNameTextBox" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
                                                        ErrorMessage="Rule must have a name" InvalidInputIndicatorID="RuleNameHelp" Display="None"></CCValidators:ConditionalRequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="RuleTypeLabel" runat="server" Text="Type" CssClass="CSSTextLabel"></asp:Label><br />
                                                    <asp:DropDownList ID="RuleTypeDropDownList" runat="server" Width="90%">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="RuleApplyTimeLabel" runat="server" Text="Apply Time"></asp:Label><br />
                                                    <asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" Width="90%">
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
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="EnabledCheckBox" runat="server" Text="Enabled" Checked="true" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="DefaultCheckBox" runat="server" Text="Default" Checked="false" />
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
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="RuleXmlTabPanel" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSDialogTabPanelContent">
                            <table width="100%" cellpadding="5" cellspacing="5">
                                <tr>
                                    <td>
                                        <asp:Label ID="SelectSampleRuleLabel" runat="server" Text="Select Sample Rule"></asp:Label><br />
                                        <asp:DropDownList ID="SampleRuleDropDownList" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
                                            Rows="16" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                    <td>
                                        <CCCommon:InvalidInputIndicator ID="InvalidRuleHint" runat="server" ImageUrl="~/images/icons/HelpSmall.png" />
                                        <CCValidators:ServerRuleValidator runat="server" ID="ServerRuleValidator" ControlToValidate="RuleXmlTextBox"
                                            InputName="Server Rule XML" InvalidInputColor="#FAFFB5" InvalidInputIndicatorID="InvalidRuleHint"
                                            ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
                                            ParamsFunction="ValidationServerRuleParams" ErrorMessage="Invalid Server Rule"
                                            Display="None" ValidationGroup="vg1" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <HeaderTemplate>
                        Rule XML
                    </HeaderTemplate>
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
            <center>
                <br />
                <table width="60%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="OKButton" runat="server" OnClick="OKButton_Click" ValidationGroup="vg1"
                                Text="Add" Width="77px" />
                        </td>
                        <td align="center">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
                <br />
            </center>
        </asp:Panel>
    </ContentTemplate>
</clearcanvas:ModalDialog>
