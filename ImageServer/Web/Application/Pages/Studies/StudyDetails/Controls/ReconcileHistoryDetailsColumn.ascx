<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common.CommandProcessor" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ReconcileHistoryDetailsColumn.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.ReconcileHistoryDetailsColumn" %>

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
    <%# String.Format("{0}", HtmlUtility.GetEnumInfo(ReconcileHistory.Action).LongDescription)%>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[Show Details]</a>
</div>

<div id="HistoryDetailsPanel" runat="server" class="TallHistoryDetailsPanel">
    <table  width="100%">
    <tr>
        <td style="border-bottom:none; padding-bottom:5px" valign="top">
            <table border="0" width="100%">
                <tr><td class="HistoryDetailsLabel" style="border-bottom:dashed 1px #c0c0c0;">Study (Snapshot):</td></tr>
                <tr><td style="border:none">
                    <div>
                        <table cellpadding="0" cellspacing="0">
                            <tr><td style="border-bottom:none">Patient ID</td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.PatientId %></pre></td></tr>
                            <tr><td style="border-bottom:none">Issuer of Patient ID</td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.IssuerOfPatientId %></pre></td></tr>
                            <tr><td style="border-bottom:none">Patient's Name</td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.Name %></pre></td></tr>
                            <tr><td style="border-bottom:none">Patient's Birth Date</td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.PatientsBirthdate %></pre></td></tr>
                            <tr><td style="border-bottom:none">Patient's Sex </td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.PatientInfo.Sex %></pre></td></tr>
                            <tr><td style="border-bottom:none">Accession Number </td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.AccessionNumber %></pre></td></tr>
                            <tr><td style="border-bottom:none">Study Date</td><td style="border-bottom:none"><pre style="padding-left:10px"><%# ReconcileHistory.ExistingStudy.StudyDate %></pre></td></tr>
                        </table>                    
                    </div>
                </td></tr>
            </table>
        </td>
        <td style="border-bottom:none" valign="top">
            <table border="0" width="100%">
                <tr><td class="HistoryDetailsLabel" style="border-bottom:dashed 1px #c0c0c0;">Reconciled Images (Snapshot):</td></tr>
                <tr><td style="border:none">
                <div>
                    <%
                        if (ReconcileHistory.ImageSetData.Fields == null ||
                            ReconcileHistory.ImageSetData.Fields.Length == 0)
                        {%>
                    N/A
                    <% }
                        else
                        { %>
                        
                        <table cellpadding="0" cellspacing="0">                        
                            <% foreach (ImageSetField field in ReconcileHistory.ImageSetData.Fields)
                               {%>
                                    <tr>
                                        <td style="border-bottom:none"><%= HtmlUtility.Encode(field.DicomTag.Name)%></td>
                                        <td style="border-bottom:none"><pre style="padding-left:10px"><%=HtmlUtility.Encode(field.Value)%></pre></td>
                                    </tr>
                             <% }%>
                         </table>
                         <%} %>
                 </div>
                </td></tr>
            </table>        
        </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:solid 1px #cccccc; padding-top:3px;">
            <% if (!ReconcileHistory.Automatic) { %>
                    <asp:Label runat="server" CssClass="HistoryDetailsLabel">Performed by : </asp:Label><%= ReconcileHistory.UserName ?? "Unknown" %>
            <% } %>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="border-top:solid 1px #cccccc; padding-top:3px;">
            <asp:Label ID="Label1" runat="server" CssClass="HistoryDetailsLabel">Changes Applied To Existing Study: </asp:Label>
            <div style="padding-top:5px;">
                <table width="100%" cellspacing="0" >
                            <tr style="color: #205F87; background: #eeeeee; padding-top: 2px; ">
                                <td style="border-top:dashed 1px #c0c0c0;"><b>Tag</b></td>
                                <td style="border-top:dashed 1px #c0c0c0;"><b>Original Value</b></td>
                                <td style="border-top:dashed 1px #c0c0c0;"><b>New Value</b></td>
                            </tr>
                            <%{
                                  foreach (BaseImageLevelUpdateCommand theCmd in ReconcileHistory.Commands)
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
            
        </td>
    </tr>
</table>

</div>

