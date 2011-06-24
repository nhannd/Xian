<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddEditDataRuleDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.AddEditDataRuleDialog" %>
<asp:ScriptManagerProxy runat="server">
    <Services>
        <asp:ServiceReference Path="DataRuleSamples.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="850px">
    <ContentTemplate>
        <asp:ValidationSummary ID="EditDataRuleValidationSummary" ShowMessageBox="false"
            ShowSummary="true" DisplayMode="SingleParagraph" EnableClientScript="true" runat="server"
            ValidationGroup="AddEditDataRuleValidationGroup" CssClass="DialogValidationErrorMessage" />
        <div class="DialogPanelContent">
            <asp:Table ID="Table2" runat="server" SkinID="NoSkin" CellSpacing="3" CellPadding="3">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="RuleNameLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleName %>"
                            CssClass="DialogTextBoxLabel"></asp:Label>
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:TextBox ID="RuleNameTextBox" runat="server" Width="50%" ValidationGroup="AddEditDataRuleValidationGroup"
                            CssClass="DialogTextBox"></asp:TextBox>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <ccAsp:InvalidInputIndicator ID="RuleNameHelp" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
                            ControlToValidate="RuleNameTextBox" InvalidInputCSS="DialogTextBoxInvalidInput"
                            ValidationGroup="AddEditDataRuleValidationGroup" Text="Rule must have a name"
                            InvalidInputIndicatorID="RuleNameHelp" Display="None" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label runat="server" Text="<%$Resources: InputLabels, Enabled %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="EnabledCheckBox" runat="server" Text="" Checked="true" ToolTip="Enable/Disable the rule"
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                    <asp:Label runat="server" Text="<%$Resources: InputLabels, ServerRuleDefaultRule %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="DefaultCheckBox" runat="server" Text="" Checked="false" ToolTip="Default rule applied if no other rules of the type apply to a DICOM message/study."
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                    <asp:Label runat="server" Text="<%$Resources: InputLabels, ServerRuleExemptRule %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <asp:CheckBox ID="ExemptRuleCheckBox" runat="server" Text="" Checked="false" ToolTip="Rule that specifies DICOM messages or studies that are exempt from the rule."
                            CssClass="DialogCheckBox" />
                    </asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="Label3" runat="server" Text="<%$Resources: InputLabels, AuthorityGroupsDataAccess %>"
                            CssClass="DialogTextBoxLabel" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:CheckBoxList ID="AuthorityGroupCheckBoxList" runat="server" TextAlign="Right"
                                RepeatColumns="1">
                            </asp:CheckBoxList>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell runat="server" />
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server" VerticalAlign="Top">
                        <asp:Label ID="SelectSampleRuleLabel" runat="server" Text="<%$Resources: InputLabels, SelectSampleRule %>"
                            CssClass="DialogTextBoxLabel"></asp:Label>
                        <br />
                        <asp:DropDownList ID="SampleRuleDropDownList" runat="server" CssClass="DialogDropDownList" />
                    </asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="left" Width="100%">
                        <div class="DialogCheckBoxList">
                            <asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
                               Height="100%" TextMode="MultiLine" CssClass="DialogTextBox" BackColor="White"></asp:TextBox>
                        </div>
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <ccAsp:InvalidInputIndicator ID="InvalidRuleHint" runat="server" SkinID="InvalidInputIndicator" />
                        <ccValidator:ServerRuleValidator runat="server" ID="DataRuleValidator" ControlToValidate="RuleXmlTextBox"
                            InputName="Server Rule XML" InvalidInputCSS="DialogTextBoxInvalidInput" InvalidInputIndicatorID="InvalidRuleHint"
                            ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
                            ParamsFunction="ValidationServerRuleParams" Text="Invalid Server Rule" Display="None"
                            ValidationGroup="AddEditDataRuleValidationGroup" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        <table width="100%" cellspacing="0" cellpadding="0">
            <tr align="right">
                <td>
                    <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>"
                            OnClick="OKButton_Click" ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click"
                            ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
