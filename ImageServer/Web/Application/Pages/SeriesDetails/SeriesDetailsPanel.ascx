<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsPanel" %>

<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>

<asp:Panel ID="Panel1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            
      <div>
  <b class="roundedCorners">
  <b class="roundedCorners1"><b></b></b>
  <b class="roundedCorners2"><b></b></b>
  <b class="roundedCorners3"></b>
  <b class="roundedCorners4"></b>
  <b class="roundedCorners5"></b></b>

  <div class="roundedCornersfg">
  
  <table cellpadding="0" cellspacing="0" width="100%">
  <tr>
  <td class="MainContentTitle">Series Details</td>
  </tr>
  <tr>
  <td class="PatientInfo"><localAsp:PatientSummaryPanel ID="PatientSummary" runat="server" /></td>
  </tr>
  
  <tr>
  <td class="StudySummaryContent">
      <table width="100%" cellpadding="2" cellspacing="0" style="background-color: #B8D9EE">
        <tr><td class="SeriesTitle">Study Summary</td></tr>
        <tr><td>
        <localAsp:StudySummaryPanel ID="StudySummary" runat="server" />
        </td></tr>
    </table>
  </td>
  </tr>
  <tr>
  <td class="SeriesDetailsContent">
    <table width="100%" cellpadding="2" cellspacing="0" style="background-color: #B8D9EE">
        <tr><td class="SeriesTitle">Series Summary</td></tr>
        <tr><td>
        <localAsp:SeriesDetailsView ID="SeriesDetails" runat="server" />
        </td></tr>
    </table>
  </tr>
  
  
  </table>
  
  </div>

  <b class="roundedCorners">
  <b class="roundedCorners5"></b>
  <b class="roundedCorners4"></b>
  <b class="roundedCorners3"></b>
  <b class="roundedCorners2"><b></b></b>
  <b class="roundedCorners1"><b></b></b></b>
</div>



        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
