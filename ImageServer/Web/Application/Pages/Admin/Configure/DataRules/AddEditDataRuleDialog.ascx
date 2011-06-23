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

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="800px">
	<ContentTemplate>	
            <asp:ValidationSummary ID="EditDataRuleValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="AddEditDataRuleValidationGroup" CssClass="DialogValidationErrorMessage" />   			
			<aspAjax:TabContainer ID="ServerPartitionTabContainer" runat="server" ActiveTabIndex="0"
				CssClass="DialogTabControl">
				<aspAjax:TabPanel ID="GeneralTabPanel" runat="server" HeaderText="GeneralTabPanel"
					TabIndex="0" CssClass="DialogTabControl">
					<ContentTemplate>
							<table id="Table1" runat="server" width="100%">
								<tr>
									<td colspan="5">
										<table width="300">
											<tr>
												<td>
													<asp:Label ID="RuleNameLabel" runat="server" Text="<%$Resources: InputLabels, ServerRuleName %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
													<asp:TextBox ID="RuleNameTextBox" runat="server" Width="285" ValidationGroup="AddEditDataRuleValidationGroup" CssClass="DialogTextBox"></asp:TextBox>
												</td>
												<td valign="bottom" align="center">
													<ccAsp:InvalidInputIndicator ID="RuleNameHelp" runat="server" SkinID="InvalidInputIndicator"/>
													<ccValidator:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
														ControlToValidate="RuleNameTextBox" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="AddEditDataRuleValidationGroup"
														Text="Rule must have a name" InvalidInputIndicatorID="RuleNameHelp" Display="None"/>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:CheckBox ID="EnabledCheckBox" runat="server" Text="<%$Resources: InputLabels, Enabled %>" Checked="true" ToolTip="Enable/Disable the rule" CssClass="DialogCheckBox"/>
									</td>
									<td>
										<asp:CheckBox ID="DefaultCheckBox" runat="server" Text="<%$Resources: InputLabels, ServerRuleDefaultRule %>" Checked="false"
											ToolTip="Default rule applied if no other rules of the type apply to a DICOM message/study." CssClass="DialogCheckBox" />
									</td>
									<td>
										<asp:CheckBox ID="ExemptRuleCheckBox" runat="server" Text="<%$Resources: InputLabels, ServerRuleExemptRule %>" Checked="false"
											ToolTip="Rule that specifies DICOM messages or studies that are exempt from the rule." CssClass="DialogCheckBox" />
									</td>
									<td></td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate><%= Titles.AdminServerRules_AddEditDialog_GeneralTabTitle%></HeaderTemplate>
				</aspAjax:TabPanel>
				<aspAjax:TabPanel ID="RuleXmlTabPanel" runat="server" HeaderText="TabPanel2">
					<ContentTemplate>
							<table width="100%" cellpadding="5" cellspacing="5">
								<tr>
									<td>
										<asp:Label ID="SelectSampleRuleLabel" runat="server" Text="<%$Resources: InputLabels, SelectSampleRule %>" CssClass="DialogTextBoxLabel"></asp:Label><br />
										<asp:DropDownList ID="SampleRuleDropDownList" runat="server" CssClass="DialogDropDownList"/>
									</td>
								</tr>
								<tr>
									<td>
										<div style="border: solid 1px #618FAD;" >
										    <asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
											    Rows="16" TextMode="MultiLine" CssClass="DialogTextArea" BackColor="White"></asp:TextBox>
                                        </div>											
									</td>
									<td>
										<ccAsp:InvalidInputIndicator ID="InvalidRuleHint" runat="server" SkinID="InvalidInputIndicator" />
										<ccValidator:ServerRuleValidator runat="server" ID="DataRuleValidator" ControlToValidate="RuleXmlTextBox"
											InputName="Server Rule XML" InvalidInputCSS="DialogTextBoxInvalidInput" InvalidInputIndicatorID="InvalidRuleHint"
											ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
											ParamsFunction="ValidationServerRuleParams" Text="Invalid Server Rule"
											Display="None" ValidationGroup="AddEditDataRuleValidationGroup"  />
									</td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate><%= Titles.AdminServerRules_AddEditDialog_RuleXMLTabTitle%>
					</HeaderTemplate>
				</aspAjax:TabPanel>
			</aspAjax:TabContainer>
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr align="right">
                    <td>
                            <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="UpdateButton" runat="server" SkinID="<%$Image:UpdateButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:AddButton%>" OnClick="OKButton_Click" ValidationGroup="AddEditDataRuleValidationGroup" OnClientClick="UpdateRuleXML()" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>" OnClick="CancelButton_Click" />
                            </asp:Panel>
                    </td>
                </tr>
            </table>
	</ContentTemplate>
</ccAsp:ModalDialog>

