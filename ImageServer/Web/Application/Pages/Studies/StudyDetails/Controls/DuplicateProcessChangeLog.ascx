<%@ Import namespace="ClearCanvas.ImageServer.Core.Data"%>
<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateProcessChangeLog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DuplicateProcessChangeLog" %>


<asp:Panel runat="server" ID="SummaryPanel">
    <asp:Label runat="server" ID="Label1">
    </asp:Label>
</asp:Panel>

<asp:Panel runat="server" ID="DetailsPanel" CssClass="HistoryDetailContainer">
    <asp:Table ID="Table2" runat="server" BorderColor="gray" BorderWidth="0px" CellPadding="0"
        CellSpacing="2" CssClass="HistoryDetailTable">
            <asp:TableRow BorderWidth="0px">
            <asp:TableCell BorderWidth="0px" VerticalAlign="top">
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr><td class="HistoryDetailsHeading">
                            <span style="padding-left:5px">Study (Snapshot):</span></td></tr>
                        <tr><td style="border:none">
                        <div>
                            <pre style="padding-left:10px">Patient ID = <%# ChangeLog.StudySnapShot.PatientInfo.PatientId%></pre>
                            <pre style="padding-left:10px">Issuer of Patient ID = <%# ChangeLog.StudySnapShot.PatientInfo.IssuerOfPatientId%></pre>
                            <pre style="padding-left:10px">Patient's Name = <%# ChangeLog.StudySnapShot.PatientInfo.Name%></pre>
                            <pre style="padding-left:10px">Patient's Birth date = <%# ChangeLog.StudySnapShot.PatientInfo.PatientsBirthdate%></pre>
                            <pre style="padding-left:10px">Patient's Sex = <%# ChangeLog.StudySnapShot.PatientInfo.Sex%></pre>
                            <pre style="padding-left:10px">Accession Number = <%# ChangeLog.StudySnapShot.AccessionNumber%></pre>
                            <pre style="padding-left:10px">Study Date = <%# ChangeLog.StudySnapShot.StudyDate%></pre>
                            <div class="DuplicateDialogSeriesPanel">
                                <table width="100%" class="DuplicateDialogSeriesTable">
                                    <tr class="DuplicateDialogSeriesTableHeader"><td>Description</td><td>Modality</td><td>Instances</td></tr>
                                    <% foreach (SeriesInformation series in ChangeLog.StudySnapShot.Series) {%>
                                            <tr>
                                                <td><%= series.SeriesDescription %></td>
                                                <td><%= series.Modality %></td>
                                                <td><%= series.NumberOfInstances %></td>
                                            </tr>
                                    <% }%>
                                </table>
                            </div>
                         </div>
                        </td></tr>
                    </table>
                </div>
            </asp:TableCell>
            <asp:TableCell BorderWidth="0px" VerticalAlign="top">
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr>
                            <td class="HistoryDetailsHeading">
                                <span style="padding-left:5px">Duplicate Images:</span>
                            </td>
                        </tr>
                        <tr><td style="border:none">
                            <div style="margin-left:2px;">
                                <%  if (ChangeLog.DuplicateDetails == null || ChangeLog.DuplicateDetails.StudyInfo==null)
                                    { %>
                                        N/A
                                <%  }
                                    else{ %>
                                        <pre style="padding-left:10px">Patient ID = <%# ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientId%></pre>
                                        <pre style="padding-left:10px">Issuer of Patient ID = <%# ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.IssuerOfPatientId%></pre>
                                        <pre style="padding-left:10px">Patient's Name = <%# ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Name%></pre>
                                        <pre style="padding-left:10px">Patient's Birth date = <%# ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientsBirthdate%></pre>
                                        <pre style="padding-left:10px">Patient's Sex = <%# ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Sex%></pre>
                                        <pre style="padding-left:10px">Accession Number = <%# ChangeLog.DuplicateDetails.StudyInfo.AccessionNumber%></pre>
                                        <pre style="padding-left:10px">Study Date = <%# ChangeLog.DuplicateDetails.StudyInfo.StudyDate%></pre>
                                        
                                        <div class="DuplicateDialogSeriesPanel">
                                            <table width="100%" class="DuplicateDialogSeriesTable">
                                                <tr class="DuplicateDialogSeriesTableHeader"><td>Description</td><td>Modality</td><td>Instances</td></tr>
                                                <% foreach(SeriesInformation series in ChangeLog.DuplicateDetails.StudyInfo.Series) {%>
                                                        <tr>
                                                            <td><%= series.SeriesDescription %></td>
                                                            <td><%= series.Modality %></td>
                                                            <td><%= series.NumberOfInstances %></td>
                                                        </tr>
                                                <% }%>
                                            </table>
                                        </div>
                                        
                                <%  } %>
                            </div>
                        </td></tr>
                    </table>
                </div>
            </asp:TableCell>
        </asp:TableRow>
            <asp:TableRow BorderWidth="0px">
                <asp:TableCell BorderWidth="0px" ColumnSpan="2">
                    <div class="HistoryDetailsSectionPanel">
                        <table border="0" width="100%">
                            <tr>
                                <td class="HistoryDetailsHeading">
                                    <span style="margin-left:5px">
                                        Action: <%# ActionDescription%>
                                    </span>
                                </td></tr>
                            <tr><td style="border:none">
                            <div style="margin-left:2px;">
                             </div>
                            </td></tr>
                        </table>
                    </div>
                </asp:TableCell>
            </asp:TableRow>
            
    </asp:Table>
</asp:Panel>
<aspAjax:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="DetailsPanel"
    AutoExpand="false" CollapseControlID="SummaryPanel" ExpandControlID="SummaryPanel"
    Collapsed="true" CollapsedText='<%# String.Format("{0} (Click for details)", ChangeLogShortDescription) %>'
    ExpandedText=" " TextLabelID="Label1">
</aspAjax:CollapsiblePanelExtender>
