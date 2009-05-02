<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.DuplicateSopDialog"
    Codebehind="DuplicateSopDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="DuplicateSopReconcileModalDialog" runat="server" Title="Duplicate SOP Reconciliation">
    <ContentTemplate>
        <aspAjax:TabContainer runat="server" ID="TabContainer"  Height="500px"  Width="950px">
            <aspAjax:TabPanel runat="server" id="OverviewTab" HeaderText="Overview" Height="100%"  >
                <ContentTemplate>                
                    <asp:Panel ID="Panel1" runat="server" CssClass="ReconcilePanel">
                        <asp:Table ID="Table1" runat="server">
                                            <asp:TableRow CssClass="ReconcileHeaderRow">
                                                <asp:TableCell >Existing Study</asp:TableCell>
                                                <asp:TableCell CssClass="Separator"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="1px" /></asp:TableCell>
                                                <asp:TableCell><span class="ConflictingStudyTitle">Duplicate SOP</span></asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <asp:TableCell ColumnSpan="3"><div class="StudyInstanceUIDMessage">Study Instance UID: <asp:Label ID="StudyInstanceUIDLabel" runat="server" Text='<%# ReconcileDetails.StudyInstanceUID %>'></asp:Label></div></asp:TableCell>
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
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="PreformattedLabel1" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.Name %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label321" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientID" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.PatientID %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label322" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientBirthDate" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.BirthDate %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label323" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.AccessionNumber %>' /></td>
                                                                    </tr>
                                                                    <tr><td class="DialogLabelBackground"><asp:Label ID="Label324" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                                                        <td>
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr><td><asp:Label ID="ExistingPatientSex" runat="server" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.Sex %>'></asp:Label></td>
                                                                                    <td><ccAsp:InvalidInputIndicator ID="InvalidInputIndicator1" runat="server" SkinID="InvalidInputIndicator" /></td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>                                                        
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label325" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.Patient.IssuerOfPatientID %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label7" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# ReconcileDetails.ExistingStudy.StudyDate %>' /></td>
                                                                    </tr>
                                                                </table>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                                                <div class="SeriesInformation">
                                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                                    <tr><td style="padding: 0px 12px 0px 4px;">
                                                                        <div class="ReconcileGridViewPanel">
                                                                    <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ExistingPatientSeriesGridView" width="400px" BackColor="white" GridLines="Horizontal" BorderColor="Transparent" AutoGenerateColumns="false">
                                                                        <Columns>
						                                                    <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
						                                                    <asp:BoundField HeaderText="Modalitiy" DataField="Modalitiy" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />                    
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
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingNameLabel" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name)? "Not Specified": ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Name%>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIDLabel" CssClass="StudyField" Text='<%# ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientId %>' /></td>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientBirthDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate)? "Not Specified":ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.PatientsBirthdate %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingAccessionNumberLabel" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.AccessionNumber)?"Not Specified":ReconcileDetails.ConflictingImageSet.StudyInfo.AccessionNumber %>' /></td>
                                                                     </tr>
                                                                     <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label6" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                                                        <td>
                                                                            <table cellpadding="0" cellspacing="0">
                                                                                <tr><td><asp:textbox ID="ConflictingPatientSex" runat="server" CssClass="StudyInfoField" BorderWidth="0" ReadOnly="true" Width="95" ValidationGroup="vg1" BorderStyle="None" BackColor="Transparent" Font-Size="14px" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex)?"Not Specified":ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.Sex %>'></asp:textbox></td>
                                                                                <td><ccAsp:InvalidInputIndicator ID="UnknownSex" runat="server" SkinID="InvalidInputIndicator" />
                                                                                    <ccValidator:RegularExpressionFieldValidator ID="PatientSexValidator" runat="server" 
                                                                                        ControlToValidate="ConflictingPatientSex" ValidationGroup="vg1" InvalidInputIndicatorID="UnknownSex"
                                                                                        ValidationExpression="M$|m$|F$|f$|O$" Text="The value used for Merge Study<br/>will be Other (O)." Display="None">
                                                                                    </ccValidator:RegularExpressionFieldValidator>
                                                                                </td>
                                                                                </tr>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label8" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingPatientIssuerOfPatientID" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId)? "Not Specified":ReconcileDetails.ConflictingImageSet.StudyInfo.PatientInfo.IssuerOfPatientId %>' /></td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="DialogLabelBackground"><asp:Label ID="Label9" runat="server" CssClass="DialogTextBoxLabel">Study Date</asp:Label></td>
                                                                        <td><ccUI:PreformattedLabel runat="server" ID="ConflictingStudyDate" CssClass="StudyField" Text='<%# String.IsNullOrEmpty(ReconcileDetails.ConflictingImageSet.StudyInfo.StudyDate)?"Not Specified":ReconcileDetails.ConflictingImageSet.StudyInfo.StudyDate %>' /></td>
                                                                    </tr>
                                                                </table>
                                                                </div>
                                                                <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                                                <div class="SeriesInformation">
                                                                <table cellpadding="0" cellspacing="0" width="100%">
                                                                    <tr><td style="padding: 0px 12px 0px 4px;">
                                                                <div class="ReconcileGridViewPanel">
                                                                    <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ConflictingPatientSeriesGridView" width="420px" BackColor="white" GridLines="Horizontal" BorderColor="Transparent" AutoGenerateColumns="false">
                                                                        <Columns>
						                                                    <asp:BoundField HeaderText="Description" DataField="SeriesDescription" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
						                                                    <asp:BoundField HeaderText="Modalitiy" DataField="Modality" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />                    
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
                                                                <table cellpadding="0" cellspacing="0" width="100%" class="ReconcileButtonsTable">
                                                                    <tr style="padding-left: 5px; padding-top: 5px;padding-bottom: 5px;">
                                                                        <td><asp:radiobutton runat="server" ID="ReplaceExistingSopRadioButton" Text=" Replace Existing SOP" GroupName="DuplicateSopDecision" Checked="true"/></td>
                                                                        <td><asp:radiobutton runat="server" ID="DeleteDuplicateRadioButton" Text=" Delete Duplicates" GroupName="DuplicateSopDecision" CssClass="ReconcileRadioButton"/></td>
                                                                    </tr>
                                                                </table>                                        
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
                    <asp:Panel runat="server" Height="100%">
                        <asp:Panel ID="Panel3" runat="server" CssClass="ReconcilePanel">
                        <table>
                            <tr>
                                <td>Study Location</td><td><asp:Label runat="server" ID="StudyLocation"></asp:Label></td>
                            </tr>
                            
                            <tr>
                                <td>Duplicate SOP Location</td><td><asp:Label runat="server" ID="DuplicateSopLocation"></asp:Label></td>
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
                    <asp:Panel ID="Panel2" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OKButton" OnClick="OKButton_Click" Enabled="false" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" Enabled="false" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
        
    </ContentTemplate>
</ccAsp:ModalDialog>

<ccAsp:MessageBox runat="server" ID="MessageBox" />
