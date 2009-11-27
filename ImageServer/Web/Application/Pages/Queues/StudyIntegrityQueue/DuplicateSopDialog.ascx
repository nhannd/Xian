<%@ Import namespace="ClearCanvas.ImageServer.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.DuplicateSopDialog"
    Codebehind="DuplicateSopDialog.ascx.cs" %>
    
<script type="text/javascript">
    Sys.Application.add_load(function()
        {
            var okButton = $find("<%= OKButton.ClientID %>");
            var useExistingRadio = $("#<%= UseExistingSopRadioButton.ClientID %>");
            var useDuplicateRadio = $("#<%= UseDuplicateRadioButton.ClientID %>");
            var deleteDuplicateRadio = $("#<%= DeleteDuplicateRadioButton.ClientID %>");
            var replaceAsIsRadio = $("#<%=ReplaceAsIsRadioButton.ClientID %>");
            
            var useExistingWarning = $("#<%=UseExistingWarningPanel.ClientID %>");
            var useDuplicateWarning = $("#<%=UseDuplicateWarningPanel.ClientID %>");
            
            var dataIsConsistent = <%= DataIsConsistent? "true":"false" %>;
            
            okButton.set_enable(false);            
            useExistingRadio.attr("checked", false);
            useDuplicateRadio.attr("checked", false);
            deleteDuplicateRadio.attr("checked", false);
            replaceAsIsRadio.attr("checked", false);
            useExistingWarning.hide();
            useDuplicateWarning.hide();
            
            replaceAsIsRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.hide();
                        useDuplicateWarning.hide();
                    }
                }
            );
            
            useExistingRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.show();
                        useDuplicateWarning.hide();
                    }
                }
            );
            
            useDuplicateRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);
                    if (!dataIsConsistent)
                    {
                        useExistingWarning.hide();
                        useDuplicateWarning.show();
                    }
                 }
            );
            
            deleteDuplicateRadio.click(
                function(ev)
                {
                    okButton.set_enable(true);    
                    useExistingWarning.hide();
                    useDuplicateWarning.hide();
                }
            );
        });
</script>

<ccAsp:ModalDialog ID="DuplicateSopReconcileModalDialog" runat="server" Title="Duplicate SOP Reconciliation">
    <ContentTemplate>       
        <aspAjax:TabContainer runat="server" ID="TabContainer"  Width="950px" ActiveTabIndex="0" CssClass="DialogTabControl">
            <aspAjax:TabPanel runat="server" id="OverviewTab" HeaderText="Overview" Height="100%" CssClass="DialogTabControl">
                <ContentTemplate>                
                    <asp:Panel ID="Panel1" runat="server" CssClass="ReconcilePanel">
                        <asp:Table ID="Table1" runat="server">
                            <asp:TableRow CssClass="ReconcileHeaderRow">
                                <asp:TableCell >Existing Study</asp:TableCell>
                                <asp:TableCell CssClass="Separator"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="1px" /></asp:TableCell>
                                <asp:TableCell><span class="ConflictingStudyTitle">Duplicate SOP</span></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell ColumnSpan="3"><div class="StudyInstanceUIDMessage">Study Instance UID: <asp:Label ID="StudyInstanceUIDLabel" runat="server" Text='<%# DuplicateEntryDetails.StudyInstanceUid %>'></asp:Label></div></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow VerticalAlign="Top">
                                <asp:TableCell>
                                    <asp:Table ID="Table2" runat="server">
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <div class="StudyInformation">
                                                 <table>
                                                    <tr>
                                                        <td width="130px" class="DialogLabelBackground"><asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="PreformattedLabel1" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.Name %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label321" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientID" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.PatientID %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label322" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientBirthDate" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.BirthDate %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label323" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.AccessionNumber %>' /></td>
                                                    </tr>
                                                    <tr><td class="DialogLabelBackground"><asp:Label ID="Label324" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                                        <td>
                                                            <table cellpadding="0" cellspacing="0">
                                                                <tr><td><asp:Label ID="ExistingPatientSex" runat="server" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.Sex %>'></asp:Label></td>
                                                                    <td><ccAsp:InvalidInputIndicator ID="InvalidInputIndicator1" runat="server" SkinID="InvalidInputIndicator" /></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>                                                        
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label325" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.Patient.IssuerOfPatientID %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ExistingStudy.StudyDate %>' /></td>
                                                    </tr>
                                                </table>
                                                </div>
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr>
                                                </table>
                                                <div class="SeriesInformation">
                                                    <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr><td style="padding: 0px 12px 0px 4px;">
                                                            <div class="ReconcileGridViewPanel" style="height:150px;">
                                                                <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ExistingPatientSeriesGridView" width="440px" AutoGenerateColumns="false">
                                                                    <Columns>
						                                                <asp:BoundField HeaderText="Number" DataField="SeriesNumber" />
		                                                                <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="left" ItemStyle-HorizontalAlign="Left">						                                                        
		                                                                    <ItemTemplate>
		                                                                        <asp:Label runat="server" ID="SeriesDescription" Text='<%# Eval("Description") %>' ToolTip='<%# Eval("SeriesInstanceUid") %>'></asp:Label>
		                                                                    </ItemTemplate>
		                                                                </asp:TemplateField>
		                                                                <asp:BoundField HeaderText="Modality" DataField="Modality" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />                    
		                                                                <asp:BoundField HeaderText="Instances" DataField="NumberOfInstances" />
		                                                            </Columns>
		                                                            <RowStyle CssClass="ReconcileSeriesGridViewRow" />
    						                                        <HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                                                </asp:GridView>
                                                            </div>
                                                        </td></tr>
                                                    </table>
                                                </div>
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Dicom Tags</div></td></tr>
                                                </table>
                                                <div class="SeriesInformation">
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                        <tr><td style="padding: 0px 12px 0px 4px;">
                                                <div class="ReconcileGridViewPanel" style="height:90px; margin-bottom: 10px;">
                                                <asp:GridView runat="server" CssClass="ReconcileComparisonResultGridView" ID="ComparisonResultGridView" Width="100%" BackColor="white" GridLines="Horizontal" BorderColor="Transparent" AutoGenerateColumns="false">
                                                <Columns>
                                                    <asp:BoundField HeaderText="Tag" DataField="TagName" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false" ItemStyle-VerticalAlign="Top" />
                                                    <asp:BoundField HeaderText="Details" DataField="Details" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                </Columns>
                                                <RowStyle CssClass="ReconcileComparisonResultGridViewRow" />
		                                        <HeaderStyle CssClass="ReconcileComparisonResultGridViewHeader" />
                                                </asp:GridView>
                                                </div>
                                                    </td></tr></table>

                                                </div>
                                           </asp:TableCell>
                                        </asp:TableRow>
                                     </asp:Table>
                                 </asp:TableCell>
                                <asp:TableCell CssClass="Separator"><asp:Image ID="Image2" runat="server" SkinID="Spacer" Width="2px" /></asp:TableCell>
                                <asp:TableCell>
                                    <asp:Table ID="Table3" runat="server">
                                        <asp:TableRow>
                                            <asp:TableCell>
                                                <div class="StudyInformation">
                                                 <table>
                                                    <tr>
                                                        <td width="130px" class="DialogLabelBackground"><asp:Label ID="Label2" runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingNameLabel" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name)? "Not Specified": DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name%>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIDLabel" CssClass="StudyField" Text='<%# DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientId %>' /></td>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientBirthDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate)? "Not Specified":DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingAccessionNumberLabel" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.AccessionNumber)?"Not Specified":DuplicateEntryDetails.ConflictingImageSet.StudyInfo.AccessionNumber %>' /></td>
                                                     </tr>
                                                     <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                                        <td>
                                                            <table cellpadding="0" cellspacing="0">
                                                                <tr><td><asp:textbox ID="ConflictingPatientSex" runat="server" CssClass="StudyInfoField" BorderWidth="0" ReadOnly="true" Width="95" ValidationGroup="DuplicateSOPValidationGroup" BorderStyle="None" BackColor="Transparent" Font-Size="14px" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex)?"Not Specified":DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex %>'></asp:textbox></td>
                                                                <td><ccAsp:InvalidInputIndicator ID="UnknownSex" runat="server" SkinID="InvalidInputIndicator" />
                                                                    <ccValidator:RegularExpressionFieldValidator ID="PatientSexValidator" runat="server" 
                                                                        ControlToValidate="ConflictingPatientSex" ValidationGroup="DuplicateSOPValidationGroup" InvalidInputIndicatorID="UnknownSex"
                                                                        ValidationExpression="M$|m$|F$|f$|O$" Text="The value used for Merge Study<br/>will be Other (O)." Display="None">
                                                                    </ccValidator:RegularExpressionFieldValidator>
                                                                </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label8" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId)? "Not Specified":DuplicateEntryDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId %>' /></td>
                                                    </tr>
                                                    <tr>
                                                        <td class="DialogLabelBackground"><asp:Label ID="Label9" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingStudyDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(DuplicateEntryDetails.ConflictingImageSet.StudyInfo.StudyDate)?"Not Specified":DuplicateEntryDetails.ConflictingImageSet.StudyInfo.StudyDate %>' /></td>
                                                    </tr>
                                                </table>
                                                </div>
                                                <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                                <div class="SeriesInformation">
                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                    <tr><td style="padding: 0px 12px 0px 4px;">
                                                <div class="ReconcileGridViewPanel" style="height:150px;">
                                                    <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ConflictingPatientSeriesGridView" width="440px" AutoGenerateColumns="false">
                                                        <Columns>
						                                    <asp:BoundField HeaderText="Number" DataField="SeriesNumber" />
		                                                    <asp:TemplateField HeaderText="Description" HeaderStyle-HorizontalAlign="left" ItemStyle-HorizontalAlign="Left">						                                                        
		                                                        <ItemTemplate>
		                                                            <asp:Label runat="server" ID="SeriesDescription" Text='<%# Eval("SeriesDescription") %>' ToolTip='<%# Eval("SeriesInstanceUid") %>'></asp:Label>
		                                                        </ItemTemplate>
		                                                    </asp:TemplateField>
		                                                    <asp:BoundField HeaderText="Modality" DataField="Modality" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />                    
		                                                    <asp:BoundField HeaderText="Instances" DataField="NumberOfInstances" />
		                                                </Columns>
		                                                <RowStyle CssClass="ReconcileSeriesGridViewRow" />
    						                            <HeaderStyle CssClass="ReconcileSeriesGridViewHeader" />
                                                    </asp:GridView>
                                                </div>
                                                    </td></tr>
                                                </table>
                                                </div>
                                           </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell style="padding: 0px 10px 10px 10px;">
                                                <asp:Panel ID="Panel2" runat="server" CssClass="ReconcileButtonsTable">
                                                    <asp:Table runat="server" ID="OptionTable" Width="100%" CellPadding="0" CellSpacing="0" style="margin-left: 5px;">
                                                        <asp:TableRow>
                                                            <asp:TableCell ColumnSpan="2"><span style="font-size: 8px;">&nbsp;</span></asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <asp:TableCell><asp:radiobutton runat="server" ID="UseExistingSopRadioButton" Text=" Use Existing Demographics" GroupName="DuplicateSopDecision" Checked="true"/></asp:TableCell>
                                                            <asp:TableCell><asp:radiobutton runat="server" ID="UseDuplicateRadioButton" Text=" Use Duplicate Demographics" GroupName="DuplicateSopDecision" Checked="false"/></asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow >
                                                            <asp:TableCell><asp:radiobutton runat="server" ID="ReplaceAsIsRadioButton" Text=" Replace As Is" GroupName="DuplicateSopDecision" Checked="false"/></asp:TableCell>
                                                            <asp:TableCell><asp:radiobutton runat="server" ID="DeleteDuplicateRadioButton" Text=" Delete Duplicates" GroupName="DuplicateSopDecision" CssClass="ReconcileRadioButton"/></asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
                                                        </asp:TableRow>                                                        
                                                        <asp:TableRow runat="server" ID="OverwritewWarningPanel" style="padding: 2px;">
                                                            <asp:TableCell ColumnSpan="2">
                                                                <asp:Panel runat="server" ID="UseExistingWarningPanel" CssClass="OverwritewWarningPanel" style="margin-right: 10px; margin-bottom: 10px;">
                                                                    WARNING: Duplicates contain information which does not match the existing study.
                                                                    This information will be lost when the existing SOPs are overwritten.
                                                                </asp:Panel>      
                                                                <asp:Panel runat="server" ID="UseDuplicateWarningPanel" CssClass="OverwritewWarningPanel" style="margin-right: 10px; margin-bottom: 10px;">
                                                                    WARNING: Duplicates contain information which does not match the existing study.
                                                                    The information in the study (and all existing images) will be replaced.
                                                                </asp:Panel>                                                                        
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </asp:Panel>                                                                                                    
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>                                    
                                </asp:TableCell>
                            </asp:TableRow>

                        </asp:Table>
                    </asp:Panel>
                </ContentTemplate>
            </aspAjax:TabPanel>
            <aspAjax:TabPanel runat="server" id="DetailsTab" HeaderText="Additional Info">
                <ContentTemplate>
                    <asp:Panel ID="Panel4" runat="server" Height="100%">
                        <asp:Panel ID="Panel5" runat="server" CssClass="AdditionalInformationPanel">
                        <table width="100%">
                            <tr>
                                <td colspan="2">
                                    <div class="AdditionalInfoSectionHeader FilesystemSectionHeader">Filesystem Locations</div>
                                </td>
                            </tr>
                            <tr>
                                <td class="DialogLabelBackground" style="margin-left:5px;"><asp:Label ID="Label10" runat="server" CssClass="DialogTextBoxLabel" Text="Study Location"></asp:Label></td>
                                <td ><asp:Label runat="server" ID="StudyLocation"></asp:Label></td>
                            </tr>
                            
                            <tr >
                                <td class="DialogLabelBackground" style="margin-left:5px;"><asp:Label ID="Label12" runat="server" CssClass="DialogTextBoxLabel" Text="Duplicate SOP Location"></asp:Label></td>
                                <td><asp:Label runat="server" ID="DuplicateSopLocation"></asp:Label></td>
                            </tr>
                        </table>                        
                        
                        </asp:Panel>
                        
                        
                    </asp:Panel>
                </ContentTemplate>
            </aspAjax:TabPanel>
        </aspAjax:TabContainer>
        
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel ID="Panel6" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OKButton" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        
    </ContentTemplate>
</ccAsp:ModalDialog>

<ccAsp:MessageBox runat="server" ID="MessageBox" />
