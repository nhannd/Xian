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
                                        <tr> <td width="140px"><asp:Label runat="server" CssClass="HeadingLabel">Patient Name:</asp:Label></td><td><asp:Label ID="ExistingNameLabel" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label321" runat="server" CssClass="HeadingLabel">Patient ID:</asp:Label></td><td><asp:Label ID="ExistingPatientID" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label322" runat="server" CssClass="HeadingLabel">Patient Birthdate:</asp:Label></td><td><asp:Label ID="ExistingPatientBirthDate" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label323" runat="server" CssClass="HeadingLabel">Accession Number:</asp:Label></td><td><asp:Label ID="ExistingAccessionNumber" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label324" runat="server" CssClass="HeadingLabel">Patient Sex:</asp:Label></td><td><asp:Label ID="ExistingPatientSex" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label325" runat="server" CssClass="HeadingLabel">Issuer of Patient ID:</asp:Label></td><td><asp:Label ID="ExistingPatientIssuerOfPatientID" runat="server"></asp:Label></td></tr>                                     
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
                                        <tr> <td width="140px"><asp:Label runat="server" CssClass="HeadingLabel">Patient Name:</asp:Label></td><td><asp:Label ID="ConflictingNameLabel" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label1" runat="server" CssClass="HeadingLabel">Patient ID:</asp:Label></td><td><asp:Label ID="ConflictingPatientID" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label2" runat="server" CssClass="HeadingLabel">Patient Birthdate:</asp:Label></td><td><asp:Label ID="ConflictingPatientBirthDate" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label3" runat="server" CssClass="HeadingLabel">Accession Number:</asp:Label></td><td><asp:Label ID="ConflictingAccessionNumber" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label4" runat="server" CssClass="HeadingLabel">Patient Sex:</asp:Label></td><td><asp:Label ID="ConflictingPatientSex" runat="server"></asp:Label></td></tr>
                                        <tr><td><asp:Label ID="Label5" runat="server" CssClass="HeadingLabel">Issuer of Patient ID:</asp:Label></td><td><asp:Label ID="ConflictingPatientIssuerOfPatientID" runat="server"></asp:Label></td></tr>                                     
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
                                        <tr><td colspan="3" class="MergeDemographicsMessage">Use demographics from: <input type="radio" /> Existing Study <input type="radio" /> Conflicting Study </td></tr>
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
