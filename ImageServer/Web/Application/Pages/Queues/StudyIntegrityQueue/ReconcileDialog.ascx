<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.ReconcileDialog"
    Codebehind="ReconcileDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ReconcileItemModalDialog" runat="server" Width="900px">
    <ContentTemplate> 
        <div class="ReconcilePanel">
            <asp:Table runat="server">
                <asp:TableRow CssClass="ReconcileHeaderRow">
                    <asp:TableCell >Existing Study</asp:TableCell>
                    <asp:TableCell CssClass="Separator"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Width="1px" /></asp:TableCell>
                    <asp:TableCell><span class="ConflictingStudyTitle">Conflicting Study</span></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="3"><div class="StudyInstanceUIDMessage">Study Instance UID: <asp:Label ID="StudyInstanceUIDLabel" runat="server"></asp:Label></div></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow VerticalAlign="Top">
                    <asp:TableCell>
                        <asp:Table runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div class="StudyInformation">
                                     <table>
                                        <tr> <td width="130px" class="DialogLabelBackground"><asp:Label runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td><td><asp:Label ID="ExistingNameLabel" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label321" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td><td><asp:Label ID="ExistingPatientID" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label322" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td><td><asp:Label ID="ExistingPatientBirthDate" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label323" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td><td><asp:Label ID="ExistingAccessionNumber" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label324" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                            <td>
                                                <table cellpadding="0" cellspacing="0">
                                                    <tr><td><asp:Label ID="ExistingPatientSex" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td>
                                                        <td><ccAsp:InvalidInputIndicator ID="InvalidInputIndicator1" runat="server" SkinID="InvalidInputIndicator" /></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>                                                        
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label325" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td><td><asp:Label ID="ExistingPatientIssuerOfPatientID" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>                                     
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
                        <asp:Table runat="server">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div class="StudyInformation">
                                     <table>
                                        <tr> <td width="130px" class="DialogLabelBackground"><asp:Label runat="server" CssClass="DialogTextBoxLabel">Patient Name</asp:Label></td><td><asp:Label ID="ConflictingNameLabel" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label1" runat="server" CssClass="DialogTextBoxLabel">Patient ID</asp:Label></td><td><asp:Label ID="ConflictingPatientID" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label2" runat="server" CssClass="DialogTextBoxLabel">Patient Birthdate</asp:Label></td><td><asp:Label ID="ConflictingPatientBirthDate" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label3" runat="server" CssClass="DialogTextBoxLabel">Accession Number</asp:Label></td><td><asp:Label ID="ConflictingAccessionNumber" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>
                                        <tr>
                                            <td class="DialogLabelBackground"><asp:Label ID="Label4" runat="server" CssClass="DialogTextBoxLabel">Patient Sex</asp:Label></td>
                                            <td>
                                                <table cellpadding="0" cellspacing="0">
                                                    <tr><td><asp:textbox ID="ConflictingPatientSex" runat="server" CssClass="ReconcileDemographicsLabel" ReadOnly="true" Width="50" ValidationGroup="vg1" BorderStyle="None" BackColor="Transparent" Font-Size="14px"></asp:textbox></td>
                                                    <td><ccAsp:InvalidInputIndicator ID="UnknownSex" runat="server" SkinID="InvalidInputIndicator" />
                                                        <ccValidator:RegularExpressionFieldValidator ID="PatientSexValidator" runat="server" 
                                                            ControlToValidate="ConflictingPatientSex" ValidationGroup="vg1" InvalidInputIndicatorID="UnknownSex"
                                                            ValidationExpression="M|m|F|f|Male|male|Female|female" Text="Unknown Value for Patient Sex.<br/>The value used for Merge Study<br/>will be Other (O)." Display="None">
                                                        </ccValidator:RegularExpressionFieldValidator>
                                                    </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr><td class="DialogLabelBackground"><asp:Label ID="Label5" runat="server" CssClass="DialogTextBoxLabel">Issuer of Patient ID</asp:Label></td><td><asp:Label ID="ConflictingPatientIssuerOfPatientID" runat="server" CssClass="ReconcileDemographicsLabel"></asp:Label></td></tr>                                     
                                    </table>
                                    </div>
                                    <table cellpadding="0" cellspacing="0" width="100%"><tr><td style="padding-left: 10px; padding-right: 10px;"><div class="SeriesTitle">Series</div></td></tr></table>
                                    <div class="SeriesInformation">
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr><td style="padding: 0px 12px 0px 4px;">
                                    <div class="ReconcileGridViewPanel">
                                        <asp:GridView runat="server" CssClass="ReconcileSeriesGridView" ID="ConflictingPatientSeriesGridView" width="400px" BackColor="white" GridLines="Horizontal" BorderColor="Transparent" AutoGenerateColumns="false">
                                            <Columns>
						                        <asp:BoundField HeaderText="Description" DataField="Description" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
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
                                <asp:TableCell style="padding: 0px 10px 6px 10px;">
                                    <table cellpadding="0" cellspacing="0" width="100%" class="ReconcileButtonsTable">
                                        <tr><td colspan="3" class="MergeDemographicsMessage">Use demographics from: <asp:radiobutton runat="server" ID="ExistingStudyButton" Text="Existing Study" Checked="true" GroupName="MergeDemographics"/>  <asp:radiobutton runat="server" ID="ConflictingStudyButton" Text="Conflicting Study" GroupName="MergeDemographics"/></td></tr>
                                        <tr class="ReconcileButtonRow">
                                            <td class="MergeStudyButton"><asp:LinkButton ID="MergeButton" runat="server" OnClick="MergeStudyButton_Click" CssClass="ReconcileButton">Merge Study</asp:LinkButton></td>
                                            <td class="CreateNewStudyButton"><asp:LinkButton ID="LinkButton1" runat="server" OnClick="NewStudyButton_Click" CssClass="ReconcileButton">Create New Study</asp:LinkButton></td>
                                            <td class="DiscardButton"><asp:LinkButton ID="LinkButton2" runat="server" OnClick="DiscardButton_Click" CssClass="ReconcileButton">Discard Study</asp:LinkButton></td></tr></table>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>                                    
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
