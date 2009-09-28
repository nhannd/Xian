<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditStudyDetailsDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditStudyDetailsDialog" %>

<ccAsp:ModalDialog ID="EditStudyModalDialog" runat="server" Width="775px" Title='<%$ Resources:Titles, EditStudyDialog %>'>
<ContentTemplate>

        <script language="javascript" type="text/javascript">
            Sys.Application.add_load(seriesPage_load);
            function seriesPage_load()
            {            
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                if (document.all) //IE6
                {
                    listbox.attachEvent('onchange', editReasonSelectionChanged);
                }
                else //Firefox
                {
                    listbox.addEventListener('onchange', editReasonSelectionChanged, false);
                }
            }
            
            function editReasonSelectionChanged()
            {
                var listbox = $get('<%= ReasonListBox.ClientID %>');
                var textbox = $get('<%= Reason.ClientID %>');
                textbox.value = listbox.options[listbox.selectedIndex].value;
                
            }
        </script>

<asp:ValidationSummary ID="EditStudyDetailsValidationSummary" ShowMessageBox="false" ShowSummary="true" DisplayMode="SingleParagraph"
EnableClientScript="true" runat="server" ValidationGroup="EditStudyValidationGroup" CssClass="EditStudyDialogErrorMessage" />
        <asp:Panel ID="Panel3" runat="server" DefaultButton="OKButton">
            <aspAjax:TabContainer ID="EditStudyDetailsTabContainer" runat="server" ActiveTabIndex="0" CssClass="EditStudyDialogTabControl" ForeColor="red">
                <aspAjax:TabPanel ID="PatientTabPanel" runat="server" HeaderText="PatientTabPanel" CssClass="EdityStudyDialogTabControl">
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
                                        <tr><td><asp:TextBox ID="PatientID" runat="server" CssClass="DialogTextBox" MaxLength="64" CausesValidation="true" ValidationGroup="EditStudyValidationGroup"></asp:TextBox>
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="PatientIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator14" runat="server" ControlToValidate="PatientID"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientIDHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Patient ID" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                 </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="GenderLabel" runat="server" Text="Gender" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                            <asp:DropDownList ID="PatientGender" runat="server" CssClass="DialogDropDownList" CausesValidation="true" ValidationGroup="EditStudyValidationGroup"  >
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
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientGenderHelp"
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
                                        <ccUI:TextBox ID="PatientBirthDate" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" Text="20010101" ></ccUI:TextBox><asp:ImageButton ID="CalendarLink" runat="server" style="padding-left: 3px; padding-right: 3px;" /><asp:LinkButton ID="ClearPatientBirthDateButton" Text="Clear" runat="server" CssClass="DialogLinkButton" />
                                        </td><td valign="bottom">
                                            <ccAsp:InvalidInputIndicator ID="PatientBirthDateHelp" runat="server" SkinID="InvalidInputIndicator" />
                                            <ccValidator:DateValidator
                                                        ID="DateValidator19" runat="server" ControlToValidate="PatientBirthDate"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientBirthDateHelp"
                                                        Text="The Patient Birth Date cannot be in the future." Display="None">
                                            </ccValidator:DateValidator>
                                            <aspAjax:MaskedEditExtender runat="server" ID="PatientBirthDateMaskExtender" MaskType="Date" Mask="99/99/9999" TargetControlID="PatientBirthDate"></aspAjax:MaskedEditExtender>
                                            <ccUI:CalendarExtender ID="PatientBirthDateCalendarExtender" runat="server" TargetControlID="PatientBirthDate" Format="MM/dd/yyyy" CssClass="Calendar" PopupButtonID="CalendarLink"></ccUI:CalendarExtender>
                                        </td></tr>
                                    </table>
                                </td>
                            <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" Text="Age" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0">
                                        <tr><td>
                                        <ccUI:TextBox ID="PatientAge" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" MaxLength="3"></ccUI:TextBox>
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
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="PatientAgeHelp"
                                                        ValidationExpression="^[^-][0-9]*$" Text="Patient Age must contain only digits and may not be negative" IgnoreEmptyValue="true" Display="None">
                                            </ccValidator:RegularExpressionFieldValidator>
                                        </td></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <HeaderTemplate>
                        Patient Information
                    </HeaderTemplate>
                </aspAjax:TabPanel>
                <aspAjax:TabPanel ID="TabPanel2" runat="server" HeaderText="TabPanel2">
                    <ContentTemplate>
                        <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee; border: solid 1px #cccccc;">
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
                                        <tr><td><asp:TextBox ID="StudyDescription" runat="server" CausesValidation="true" MaxLength="64" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" />
                                    </td>
                                    <td valign="bottom">
                                        <ccAsp:InvalidInputIndicator ID="StudyDescriptionHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator1" runat="server" ControlToValidate="StudyDescription"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDescriptionHelp"
                                                        ValidationExpression="^([^\\]){0,64}$" Text="Invalid Study Description" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                    </td></tr></table>
                                </td>
                                </tr>
                                <tr>
                                <td class="DialogLabelBackground"><asp:Label ID="Label12" runat="server" Text="Accession #" CssClass="DialogTextBoxLabel" /></td>
                                <td>
                                    <table cellpadding="0" cellspacing="0"><tr><td><asp:TextBox ID="AccessionNumber" runat="server" MaxLength="16" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom"><ccAsp:InvalidInputIndicator ID="AccessionNumberHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator2" runat="server" ControlToValidate="AccessionNumber"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="AccessionNumberHelp"
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
                                        <ccValidator:ConditionalRequiredFieldValidator runat="server" ControlToValidate="StudyDate" Text="Study Date is required" Display="none" InvalidInputIndicatorID="StudyDateHelp" InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" RequiredWhenChecked="False"/>
                                        <ccValidator:DateValidator
                                                        ID="DateValidator1" runat="server" ControlToValidate="StudyDate"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDateHelp"
                                                        Text="The Study Date cannot be in the future." Display="None">
                                        </ccValidator:DateValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator16" runat="server" ControlToValidate="StudyTimeHours"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^(0*[1-9]|1[0-2])$" IgnoreEmptyValue="true" Text="Invalid Study Time" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator17" runat="server" ControlToValidate="StudyTimeMinutes"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDateHelp"
                                            ValidationExpression="^([0-5][0-9])*$" Text="Invalid Study Time" Display="None">
                                        </ccValidator:RegularExpressionFieldValidator>
                                        <ccValidator:RegularExpressionFieldValidator
                                            ID="RegularExpressionFieldValidator18" runat="server" ControlToValidate="StudyTimeSeconds"
                                            InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyDateHelp"
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
                                    <tr><td><asp:TextBox ID="StudyID" runat="server" CausesValidation="true" ValidationGroup="EditStudyValidationGroup" MaxLength="16" CssClass="DialogTextBox" /></td>
                                    <td valign="bottom">
                                       <ccAsp:InvalidInputIndicator ID="StudyIDHelp" runat="server" SkinID="InvalidInputIndicator" />
                                        <ccValidator:RegularExpressionFieldValidator
                                                        ID="RegularExpressionFieldValidator3" runat="server" ControlToValidate="StudyID"
                                                        InvalidInputCSS="DialogTextBoxInvalidInput" ValidationGroup="EditStudyValidationGroup" InvalidInputIndicatorID="StudyIDHelp"
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
            
            <div id="ReasonPanel" class="EditStudyReasonPanel">
                <table border="0">
                    
                    <tr valign="top">
                        <td>
                            <asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel" Text="Reason:"></asp:Label>                            
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ReasonListBox" style="font-family: Arial, Sans-Serif; font-size: 14px;"/>                                        
                        </td>
                   </tr>
                   <tr>
                        <td valign="top">
                            <asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel" 
                                            Text='Comment:'></asp:Label> 
                             
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr valign="top">
                                    <td>
                                        <asp:TextBox  Width="400px" Rows="3" ID="Reason" runat="server" TextMode="MultiLine" style="font-family: Arial, Sans-Serif; font-size: 14px;" />                                            
                                    </td>
                                    <td valign="middle" style="padding-left: 8px;">
                                        <ccAsp:InvalidInputIndicator ID="InvalidReasonIndicator" runat="server" SkinID="InvalidInputIndicator" />
                                    </td>

                                </tr>
                            </table>
                            
                            
                        </td>
                    </tr>
                    <tr id="ReasonSavePanel" runat="server">
                        <td>
                            <asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel" 
                                                Text="Save reason as:"></asp:Label> 
                                 
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="SaveReasonAsName" style="font-family: Arial, Sans-Serif; font-size: 14px;"/>
                        </td>
                </tr>
                </table>
            </div>                
        </div>
        </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="UpdateButton" OnClick="OKButton_Click" ValidationGroup="EditStudyValidationGroup" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                        </asp:Panel>

                    </td>
                </tr>
            </table>

       <ccValidator:ConditionalRequiredFieldValidator ID="ReasonValidator" runat="server"
                                                ControlToValidate="Reason" InvalidInputIndicatorID="InvalidReasonIndicator" 
                                                ValidationGroup='EditStudyValidationGroup'
                                                Text="You must specify the reason for editing the studies for future auditing purposes." Display="None" InvalidInputCSS="DialogTextBoxInvalidInput"></ccValidator:ConditionalRequiredFieldValidator>
</ContentTemplate>
</ccAsp:ModalDialog>