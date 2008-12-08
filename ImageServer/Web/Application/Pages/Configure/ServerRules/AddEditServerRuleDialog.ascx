<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AddEditServerRuleDialog.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.ServerRules.AddEditServerRuleDialog" %>

<asp:ScriptManagerProxy runat="server">
	<Services>
		<asp:ServiceReference Path="ServerRuleSamples.asmx" />
	</Services>
</asp:ScriptManagerProxy>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="500px">
	<ContentTemplate>
            <asp:ValidationSummary ID="EditServerRuleValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
                EnableClientScript="true" runat="server" ValidationGroup="vg1" CssClass="DialogValidationErrorMessage" />   			
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
													<asp:Label ID="RuleNameLabel" runat="server" Text="Name" CssClass="DialogTextBoxLabel"></asp:Label><br />
													<asp:TextBox ID="RuleNameTextBox" runat="server" Width="285" ValidationGroup="vg1" CssClass="DialogTextBox"></asp:TextBox>
												</td>
												<td valign="bottom" align="center">
													<ccAsp:InvalidInputIndicator ID="RuleNameHelp" runat="server" SkinID="InvalidInputIndicator"/>
													<ccValidator:ConditionalRequiredFieldValidator ID="RuleNameValidator" runat="server"
														ControlToValidate="RuleNameTextBox" InvalidInputColor="#FAFFB5" ValidationGroup="vg1"
														Text="Rule must have a name" InvalidInputIndicatorID="RuleNameHelp" Display="None"/>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<table width="100%">
											<tr>
												<td>
													<asp:Label ID="RuleTypeLabel" runat="server" Text="Type" CssClass="DialogTextBoxLabel"/><br />
													<asp:DropDownList ID="RuleTypeDropDownList" runat="server" Width="125" CssClass="DialogDropDownList"/>
												</td>
											</tr>
										</table>
									</td>
									<td colspan="2">
										<table width="100%">
											<tr>
												<td>
													<asp:Label ID="RuleApplyTimeLabel" runat="server" Text="Apply Time" CssClass="DialogTextBoxLabel"/><br />
													<asp:DropDownList ID="RuleApplyTimeDropDownList" runat="server" Width="50%" CssClass="DialogDropDownList"/>
												</td>
												<td></td>
											</tr>
										</table>
									</td>
									<td></td>
								</tr>
								<tr>
									<td colspan="2">
										<asp:CheckBox ID="EnabledCheckBox" runat="server" Text="Enabled" Checked="true" ToolTip="Enable/Disable the rule" CssClass="DialogCheckBox"/>
									</td>
									<td>
										<asp:CheckBox ID="DefaultCheckBox" runat="server" Text="Default Rule" Checked="false"
											ToolTip="Default rule applied if no other rules of the type apply to a DICOM message/study." CssClass="DialogCheckBox" />
									</td>
									<td>
										<asp:CheckBox ID="ExemptRuleCheckBox" runat="server" Text="Exempt Rule" Checked="false"
											ToolTip="Rule that specifies DICOM messages or studies that are exempt from the rule." CssClass="DialogCheckBox" />
									</td>
									<td></td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate>
						General
					</HeaderTemplate>
				</aspAjax:TabPanel>
				<aspAjax:TabPanel ID="RuleXmlTabPanel" runat="server" HeaderText="TabPanel2">
					<ContentTemplate>
							<table width="100%" cellpadding="5" cellspacing="5">
								<tr>
									<td>
										<asp:Label ID="SelectSampleRuleLabel" runat="server" Text="Select Sample Rule" CssClass="DialogTextBoxLabel"></asp:Label><br />
										<asp:DropDownList ID="SampleRuleDropDownList" runat="server" CssClass="DialogDropDownList"/>
									</td>
								</tr>
								<tr>
									<td>
										<div style="border: solid 1px #618FAD;" >
										<asp:TextBox ID="RuleXmlTextBox" runat="server" EnableViewState="true" Width="100%"
											Rows="16" TextMode="MultiLine" CssClass="DialogTextArea" onfocus="HighlightXML()"></asp:TextBox>
</div>											
									</td>
									<td>
										<ccAsp:InvalidInputIndicator ID="InvalidRuleHint" runat="server" SkinID="InvalidInputIndicator" />
										<ccValidator:ServerRuleValidator runat="server" ID="ServerRuleValidator" ControlToValidate="RuleXmlTextBox"
											InputName="Server Rule XML" InvalidInputColor="#FAFFB5" InvalidInputIndicatorID="InvalidRuleHint"
											ServicePath="/Services/ValidationServices.asmx" ServiceOperation="ValidateServerRule"
											ParamsFunction="ValidationServerRuleParams" Text="Invalid Server Rule"
											Display="None" ValidationGroup="vg1" />
									</td>
								</tr>
							</table>
					</ContentTemplate>
					<HeaderTemplate>
						Rule XML
					</HeaderTemplate>
				</aspAjax:TabPanel>
			</aspAjax:TabContainer>
            <table width="100%" cellspacing="0" cellpadding="0">
                <tr align="right">
                    <td>
                            <asp:Panel ID="Panel3" runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="AddButton" OnClick="OKButton_Click" ValidationGroup="vg1" />
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                    </td>
                </tr>
            </table>
	</ContentTemplate>
</ccAsp:ModalDialog>
