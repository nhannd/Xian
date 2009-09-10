<%@ Import namespace="ClearCanvas.ImageServer.Core.Validation"%>
<%@ Import namespace="ClearCanvas.ImageServer.Services.WorkQueue"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueAlertContextDataView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.WorkQueueAlertContextDataView" %>

<%  WorkQueueAlertContextData data = this.Alert.ContextData as WorkQueueAlertContextData;
    String viewWorkQueueUrl = HtmlUtility.ResolveWorkQueueDetailsUrl(Page, data.WorkQueueItemKey);
    String viewStudyUrl = data.StudyInfo != null? HtmlUtility.ResolveStudyDetailsUrl(Page, data.StudyInfo.ServerAE, data.StudyInfo.StudyInstaneUid):null;
    
%>

<div >
<table class="WorkQueueAlertStudyTable" cellspacing="0" cellpadding="0">
<% if (data.StudyInfo!=null) { %>
<tr><td style="font-weight: bold; color: #336699">Partition:</td><td><%= data.StudyInfo.ServerAE %>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699">Patient's Name:</td><td><pre><%= data.StudyInfo.PatientsName%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699">Patient ID:</td><td><pre><%= data.StudyInfo.PatientsId %>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699">Study Instance UID:</td><td><%= data.StudyInfo.StudyInstaneUid%>&nbsp;</td></tr>
<tr><td style="font-weight: bold; color: #336699">Accession #:</td><td><pre><%= data.StudyInfo.AccessionNumber%>&nbsp;</pre></td></tr>
<tr><td style="font-weight: bold; color: #336699">Study Date:</td><td><pre><%= data.StudyInfo.StudyDate%>&nbsp;</pre></td></tr>
<%} else {%>
<tr><td>There is no study information for this item.</td></tr>
<%} %>

</table>

<table cellpadding="0" cellspacing="0" style="margin-top: 3px;">
    <tr >
        <% if (data.StudyInfo!=null){%>
        <td><a href='<%=viewStudyUrl%>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;">View Study</a></td>
        <td style="font-weight: bold; color: #336699;">|</td>
        <%}%>
        <td><a href='<%= viewWorkQueueUrl %>' target="_blank" style="color: #6699CC; text-decoration: none; font-weight: bold;">View Work Queue</a></td>
    </tr>
</table>

</div>
