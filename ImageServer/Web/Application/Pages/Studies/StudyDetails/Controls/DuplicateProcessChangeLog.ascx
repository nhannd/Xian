<%@ Import namespace="ClearCanvas.ImageServer.Core.Data"%>
<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateProcessChangeLog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DuplicateProcessChangeLog" %>


<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=HistoryDetailsPanel.ClientID%>").hide();
        $("#<%=ShowHideDetails.ClientID%>").click(function() {
            if ($("#<%=ShowHideDetails.ClientID%>").text() == "[Show Details]") {
                $("#<%=HistoryDetailsPanel.ClientID%>").show();
                $("#<%=ShowHideDetails.ClientID%>").text("[Hide Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "bold");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "5px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");                
            } else {
                $("#<%=HistoryDetailsPanel.ClientID%>").hide();
                $("#<%=ShowHideDetails.ClientID%>").text("[Show Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "normal");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "0px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");                
            }
            return false;
        });
    });

</script>

<div id="SummaryPanel" runat="server">
    <%# String.Format("{0}", ChangeLogShortDescription)%>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[Show Details]</a>
</div>

<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table width="100%" border="0"  cellpadding="5">
        <tr>
            <td>
                <table border="0" width="100%">
                <tr><td class="HistoryDetailsLabel">
                    <span style="padding-left:5px">Study (Snapshot):</span>
                </td></tr>
                <tr><td style="border:none">
                    <div>
                    <pre style="padding-left:10px">Patient ID = <%= ChangeLog.StudySnapShot.PatientInfo.PatientId%></pre>
                    <pre style="padding-left:10px">Issuer of Patient ID = <%= ChangeLog.StudySnapShot.PatientInfo.IssuerOfPatientId%></pre>
                    <pre style="padding-left:10px">Patient's Name = <%= ChangeLog.StudySnapShot.PatientInfo.Name%></pre>
                    <pre style="padding-left:10px">Patient's Birth date = <%= ChangeLog.StudySnapShot.PatientInfo.PatientsBirthdate%></pre>
                    <pre style="padding-left:10px">Patient's Sex = <%= ChangeLog.StudySnapShot.PatientInfo.Sex%></pre>
                    <pre style="padding-left:10px">Accession Number = <%= ChangeLog.StudySnapShot.AccessionNumber%></pre>
                    <pre style="padding-left:10px">Study Date = <%= ChangeLog.StudySnapShot.StudyDate%></pre>
                    <div class="DuplicateDialogSeriesPanel">
                        <table width="100%" class="DuplicateDialogSeriesTable">
                            <tr style="background: #e0e0e0;"><td>Description</td><td>Modality</td><td>Instances</td></tr>
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
            </td>
        
            <td>
                <table border="0" width="100%">
                <tr>
                    <td class="HistoryDetailsLabel">
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
                                <pre style="padding-left:10px">Patient ID = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientId%></pre>
                                <pre style="padding-left:10px">Issuer of Patient ID = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.IssuerOfPatientId%></pre>
                                <pre style="padding-left:10px">Patient's Name = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Name%></pre>
                                <pre style="padding-left:10px">Patient's Birth date = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.PatientsBirthdate%></pre>
                                <pre style="padding-left:10px">Patient's Sex = <%= ChangeLog.DuplicateDetails.StudyInfo.PatientInfo.Sex%></pre>
                                <pre style="padding-left:10px">Accession Number = <%= ChangeLog.DuplicateDetails.StudyInfo.AccessionNumber%></pre>
                                <pre style="padding-left:10px">Study Date = <%= ChangeLog.DuplicateDetails.StudyInfo.StudyDate%></pre>
                                
                                <div class="DuplicateDialogSeriesPanel">
                                    <table width="100%" class="DuplicateDialogSeriesTable">
                                        <tr  style="background: #e0e0e0;"><td>Description</td><td>Modality</td><td>Instances</td></tr>
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
            </td>
        </tr>
    </table>
            
    <table border="0" width="100%">
                <tr>
                    <td class="HistoryDetailsLabel">
                        <span style="margin-left:5px">
                            Changes Applied:
                        </span>
                    </td></tr>
                <tr><td style="border:none">
                <div style="margin-left:2px; padding-left:5px;">
                    
                    
                    <table width="100%" cellspacing="0" >
                        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
                            <td>
                                <b>Tag</b>
                            </td>
                            <td>
                                <b>Original Value</b>
                            </td>
                            <td>
                                <b>New Value</b>
                            </td>
                        </tr>
                        <%{
                              foreach (BaseImageLevelUpdateCommand theCmd in ChangeLog.StudyUpdateCommands)
                              {
                                  if (theCmd != null)
                                  { %><tr style="background: #fefefe">
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.TagPath.Tag) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.OriginalValue) %></pre>
                                                      </td>
                                                      <td style="border-bottom: solid 1px #dddddd">
                                                          <pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.Value!=null? theCmd.UpdateEntry.Value.ToString(): "") %></pre>
                                                      </td>
                                                  </tr>
                                <%} %>
                            <%}%>
                        <%}%>
                    </table>
                 </div>
                </td></tr>
            </table>
</div>
