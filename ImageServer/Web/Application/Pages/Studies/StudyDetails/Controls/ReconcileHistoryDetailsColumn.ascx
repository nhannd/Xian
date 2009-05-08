<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common.CommandProcessor" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ReconcileHistoryDetailsColumn.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.ReconcileHistoryDetailsColumn" %>
<asp:Panel runat="server" ID="SummaryPanel">
    <asp:Label runat="server" ID="Label1">
    </asp:Label>
</asp:Panel>
<asp:Panel runat="server" ID="DetailsPanel" CssClass="HistoryDetailContainer">
    <asp:Table ID="Table1" runat="server" BorderColor="gray" BorderWidth="0px" CellPadding="0"
        CellSpacing="0" CssClass="HistoryDetailTable">
        <asp:TableRow BorderWidth="0px">
            <asp:TableCell BorderWidth="0px" VerticalAlign="top">
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr><td class="HistoryDetailsHeading">
                            <span style="padding-left:5px">Study (Snapshot):</span></td></tr>
                        <tr><td style="border:none">
                        <div>
                            <pre style="padding-left:10px">Patient ID = <%# ReconcileHistory.ExistingStudy.PatientInfo.PatientId %></pre>
                            <pre style="padding-left:10px">Issuer of Patient ID = <%# ReconcileHistory.ExistingStudy.PatientInfo.IssuerOfPatientId %></pre>
                            <pre style="padding-left:10px">Patient's Name = <%# ReconcileHistory.ExistingStudy.PatientInfo.Name %></pre>
                            <pre style="padding-left:10px">Patient's Birth date = <%# ReconcileHistory.ExistingStudy.PatientInfo.PatientsBirthdate %></pre>
                            <pre style="padding-left:10px">Patient's Sex = <%# ReconcileHistory.ExistingStudy.PatientInfo.Sex %></pre>
                            <pre style="padding-left:10px">Accession Number = <%# ReconcileHistory.ExistingStudy.AccessionNumber %></pre>
                            <pre style="padding-left:10px">Study Date = <%# ReconcileHistory.ExistingStudy.StudyDate %></pre>
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
                                <span style="padding-left:5px">Reconciled Images:</span>
                            </td>
                        </tr>
                        <tr><td style="border:none">
                            <div style="margin-left:2px;">
                                <%  if (ReconcileHistory.ImageSetData.Fields == null || ReconcileHistory.ImageSetData.Fields.Length == 0) { %>
                                        N/A
                                <%  }
                                    else foreach (ImageSetField field in ReconcileHistory.ImageSetData.Fields) { %>
                                        <pre  style="padding-left:10px"><%= HtmlUtility.Encode(field.DicomTag.Name)%> = <%= field.Value %></pre>
                                <%  } %>
                            </div>
                        </td></tr>
                    </table>
                </div>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow BorderWidth="0px">
            <asp:TableCell ColumnSpan="2" BorderWidth="0px">
                <br />
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr><td class="HistoryDetailsHeading">
                            <span style="margin-left:5px">
                                Action (<%# ReconcileHistory.Automatic ? "Auto" : "<b>Manual</b>" %>):
                                
                                <%# HtmlUtility.GetEnumInfo(ReconcileHistory.Action).LongDescription%>
                                <%  if (!String.IsNullOrEmpty(ReconcileHistory.Description))
                                    {%>
                                        (<%# ReconcileHistory.Description %>)
                                <%  }%>
                                </span>
                            </td></tr>
                        <tr><td style="border:none">
                        <div style="margin-left:2px;">
                            <% if (ReconcileHistory.Commands == null || ReconcileHistory.Commands.Count == 0)
                               {%>
                                <pre  style="padding-left:10px">Study was not changed.</pre>
                            <%}
                              else
                              {
                                  foreach (BaseImageLevelUpdateCommand cmd in ReconcileHistory.Commands)
                                  {%>
                                        <pre  style="padding-left:10px"><%= HtmlUtility.Encode(cmd.ToString()) %></pre>
                                <%}%>
                            <%}%>
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
    Collapsed="true" CollapsedText='<%# String.Format("{0} (Click for details)", HtmlUtility.GetEnumInfo(ReconcileHistory.Action).LongDescription)%>'
    ExpandedText=" " TextLabelID="Label1">
</aspAjax:CollapsiblePanelExtender>
