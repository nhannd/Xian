<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditStudyDetailsDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditStudyDetailsDialog" %>

<ccAsp:ModalDialog ID="EditStudyModalDialog" runat="server" Width="775px">
<ContentTemplate>
<asp:ValidationSummary ID="EditStudyDetailsValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
EnableClientScript="true" runat="server" ValidationGroup="vg1" CssClass="EditStudyDialogErrorMessage" />
        <asp:Panel ID="Panel3" runat="server" DefaultButton="OKButton">
            <aspAjax:TabContainer ID="EditStudyDetailsTabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl" ForeColor="red">
                <aspAjax:TabPanel ID="PatientTabPanel" runat="server" HeaderText="PatientTabPanel" CssClass="DialogTabControl">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee; border: solid 1px #cccccc;">
                            
                            <tr>
                                <td valign="top" class="DialogLabelBackground"><asp:Label ID="Label6" runat="server" Text="Patient Name" CssClass="DialogTextBoxLabel" /></td>
                                <td><ccAsp:PersonNameInputPanel runat="server" ID="PatientNamePanel"  Required="true"/></td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label2" runat="server" Text="Patient ID" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                <table cellpadding="0" cellspacing="0">
                                        <tr><td><asp:TextBox ID="PatientID" runat="server" CssClass="DialogTextBox" MaxLength="64" CausesValidation="true" ValidationGroup="vg1"></asp:TextBox>
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="PatientIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator14" runat="server" ControlToValidate="PatientID"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PatientIDHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Patient ID" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                 </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="GenderLabel" runat="server" Text="Gender" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                            <asp:DropDownList ID="PatientGender" runat="server" CssClass="DialogDropDownList" CausesValidation="true" ValidationGroup="vg1">
                                                <asp:ListItem Selected="True" Text=" " Value=" " />
                                                <asp:ListItem Text="Male" Value="M" />
                                                <asp:ListItem Text="Female" Value="F" />
                                                <asp:ListItem Text="Other" Value="O" />
                                            </asp:DropDownList>
                                        </td>
                                        <td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientGenderHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator15" runat="server" ControlToValidate="PatientGender"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PatientGenderHelp"
                                                        ValidationExpression="M|F|O" Text="Patient Gender is a required field" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td>
                                        </tr>
                                     </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label1" runat="server" Text="Date of Birth" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                        <ccUI:TextBox ID="PatientBirthDate" runat="server" ReadOnly="true" CausesValidation="true" ValidationGroup="vg1" CssClass="DialogTextBox" ></ccUI:TextBox><asp:LinkButton ID="ClearPatientBirthDateButton" Text="Clear" runat="server" CssClass="DialogLinkButton" />
                                        </td><td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientBirthDateHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:DateValidator
                                                        ID="DateValidator19" runat="server" ControlToValidate="PatientBirthDate"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PatientBirthDateHelp"
                                                        Text="The Patient Birth Date cannot be in the future." Display="None">
                                            </ccValidator:DateValidator>
                                        </td></tr>
                                    </table>
                                </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" Text="Age" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                        <ccUI:TextBox ID="PatientAge" runat="server" CausesValidation="true" ValidationGroup="vg1" CssClass="DialogTextBox" MaxLength="3"></ccUI:TextBox>
                                        <asp:DropDownList ID="PatientAgePeriod" runat="server" CssClass="DialogDropDownList">
                                            <asp:ListItem Value="Y">Years</asp:ListItem>
                                            <asp:ListItem Value="M">Months</asp:ListItem>
                                            <asp:ListItem Value="W">Weeks</asp:ListItem>
                                            <asp:ListItem Value="D">Days</asp:ListItem>
                                        </asp:DropDownList>
                                        </td><td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientAgeHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="PatientAgeValidator" runat="server" ControlToValidate="PatientAge"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PatientAgeHelp"
                                                        ValidationExpression="^[^-][0-9]*$" Text="Patient Age must contain only digits and may not be negative" IgnoreEmptyValue="true" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <ccUI:CalendarExtender ID="PatientBirthDateCalendarExtender" runat="server" TargetControlID="PatientBirthDate" CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </ContentTemplate>
                    <HeaderTemplate>
                        Patient Information
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee; border: solid 1px #cccccc;">
                            <%--<tr>
                            <td><asp:Image ID="Image1" runat="server" SkinID="spacer" Height="6" /></td>
                                <td rowspan="2">
                                    <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label16" runat="server" Text="Title" CssClass="DialogTextBoxLabel" /><br />
                                            <asp:TextBox ID="PhysicianTitle" runat="server" CssClass="DialogTextBox" />
                                        </td>
                                        <td><asp:Label ID="Label17" runat="server" Text="Given Name" CssClass="DialogTextBoxLabel" /><br /><asp:TextBox ID="PhysicianGivenName" runat="server" MaxLength="64" CssClass="DialogTextBox" /></td>
                                        <td><asp:Label ID="Label18" runat="server" Text="Middle Name" CssClass="DialogTextBoxLabel" /><br /><asp:TextBox ID="PhysicianMiddleName" runat="server" MaxLength="64" CssClass="DialogTextBox" /></td>
                                        <td><asp:Label ID="Label19" runat="server" Text="Last Name" CssClass="DialogTextBoxLabel" /><br /><asp:TextBox ID="PhysicianLastName" runat="server" MaxLength="64" CssClass="DialogTextBox" /></td>
                                        <td><asp:Label ID="Label20" runat="server" Text="Suffix" CssClass="DialogTextBoxLabel" /><br /><asp:TextBox ID="PhysicianSuffix" runat="server" MaxLength="64" CssClass="DialogTextBox" /></td>
                                        <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="PhysiciansNameHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator9" runat="server" ControlToValidate="PhysicianTitle"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PhysiciansNameHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Physician Name. Each field may contain up to 64 characters, excluiding &quot;\\&quot;" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator10" runat="server" ControlToValidate="PhysicianGivenName"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PhysiciansNameHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Physician Name. Each field may contain up to 64 characters, excluiding &quot;\\&quot;" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator11" runat="server" ControlToValidate="PhysicianMiddleName"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PhysiciansNameHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Physician Name. Each field may contain up to 64 characters, excluiding &quot;\\&quot;" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator12" runat="server" ControlToValidate="PhysicianLastName"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PhysiciansNameHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Physician Name. Each field may contain up to 64 characters, excluiding &quot;\\&quot;" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                            <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator13" runat="server" ControlToValidate="PhysicianSuffix"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="PhysiciansNameHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Physician Name. Each field may contain up to 64 characters, excluiding &quot;\\&quot;." Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td>
                                    </tr>                                    
                                    </table>
                                </td>
                            </tr>--%>
                            <tr>
                                <td valign="top" class="DialogLabelBackground"><asp:Label ID="Label21" runat="server" Text="Referring Physician" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <ccAsp:PersonNameInputPanel runat="server" ID="ReferringPhysicianNamePanel" Required="false"/>
                                </td>
                            </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label14" runat="server" Text="Study Description" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td><asp:TextBox ID="StudyDescription" runat="server" CausesValidation="true" MaxLength="64" ValidationGroup="vg1" CssClass="DialogTextBox" />
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="StudyDescriptionHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator1" runat="server" ControlToValidate="StudyDescription"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyDescriptionHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Study Description" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                </td>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label12" runat="server" Text="Accession #" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0"><tr><td><asp:TextBox ID="AccessionNumber" runat="server" MaxLength="16" CausesValidation="true" ValidationGroup="vg1" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom"><ccAsp:InvalidInputIndicator ID="AccessionNumberHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator2" runat="server" ControlToValidate="AccessionNumber"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="AccessionNumberHelp"
                                                        ValidationExpression="^([^\\]){0,16}$" Text="Invalid Accession Number" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator></td></tr></table>
                                </td>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label11" runat="server" Text="Study Date/Time" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                        <td>
                                            <ccUI:TextBox ID="StudyDate" runat="server" CausesValidation="true" CssClass="DialogTextBox" ReadOnly="true" /><ccUI:TextBox ID="StudyTimeHours" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" /><span style="color: black">:</span><asp:TextBox ID="StudyTimeMinutes" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" /><span style="color:Black ">:</span><asp:TextBox ID="StudyTimeSeconds" runat="server" CausesValidation="true" CssClass="DialogTextBox" Width="17" MaxLength="2" /><asp:DropDownList ID="StudyTimeAmPm" runat="server" CausesValidation="true" CssClass="DialogDropDownList" ><asp:ListItem Selected="True" Text="AM" Value="AM" /><asp:ListItem Text="PM" Value="PM" /></asp:DropDownList><asp:LinkButton ID="ClearStudyDateTimeButton" Text="Clear" runat="server" CssClass="DialogLinkButton" />
                                        </td>
                                        <td>
                                        <ccAsp:InvalidInputIndicator ID="StudyDateHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:ConditionalRequiredFieldValidator runat="server" ControlToValidate="StudyDate" Text="Study Date is required" Display="none" InvalidInputIndicatorID="StudyDateHelp" InvalidInputColor="#FAFFB5" ValidationGroup="vg1" RequiredWhenChecked="False"/>
                                        <ccValidator:DateValidator
                                                        ID="DateValidator1" runat="server" ControlToValidate="StudyDate"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyDateHelp"
                                                        Text="The Study Date cannot be in the future." Display="None">
                                        </ccValidator:DateValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator16" runat="server" ControlToValidate="StudyTimeHours"
                                            InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^(0*[1-9]|1[0-2])$" IgnoreEmptyValue="true" Text="Invalid Study Time" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator17" runat="server" ControlToValidate="StudyTimeMinutes"
                                            InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^([0-5][0-9])*$" Text="Invalid Study Time" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator18" runat="server" ControlToValidate="StudyTimeSeconds"
                                            InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^([0-5][0-9])*$" Text="Invalid Study Time" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        </td>
                                        </tr>
                                        </table>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label13" runat="server" Text="Study ID" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                    <tr><td><asp:TextBox ID="StudyID" runat="server" CausesValidation="true" ValidationGroup="vg1" MaxLength="16" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom">
                                       <ccAsp:InvalidInputIndicator ID="StudyIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator3" runat="server" ControlToValidate="StudyID"
                                                        InvalidInputColor="#FAFFB5" ValidationGroup="vg1" InvalidInputIndicatorID="StudyIDHelp"
                                                        ValidationExpression="^([^\\]){0,16}$" Text="Invalid Study ID" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                </td>
                                </tr>
                             </table>
                             <ccUI:CalendarExtender ID="StudyDateCalendarExtender" runat="server" TargetControlID="StudyDate"
                                CssClass="Calendar">
                            </ccUI:CalendarExtender>
                    </ContentTemplate>
                    <HeaderTemplate>
                        Study Information
                    </HeaderTemplate>
                </aspAjax:TabPanel>
            </aspAjax:TabContainer>
        </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="UpdateButton" OnClick="OKButton_Click" ValidationGroup="vg1" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                        </asp:Panel>

                    </td>
                </tr>
            </table>
</ContentTemplate>
</ccAsp:ModalDialog>