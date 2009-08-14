<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails.SeriesDetailsPanel" %>

<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
        <ContentTemplate>
              
  <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="MainContentTitle">Series Details</td>
  </tr>
    
  <tr>
  <td class="PatientInfo"><localAsp:PatientSummaryPanel ID="PatientSummary" runat="server" /></td>
  </tr>
  
  <tr><td style="background-color: #3D98D1"><asp:Image runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
      <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle">Study Summary</td></tr>
        <tr><td>
        <localAsp:StudySummaryPanel ID="StudySummary" runat="server" />
        </td></tr>
    </table>
  </td>
  </tr>
  
  <tr><td style="background-color: #3D98D1"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td>
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle">Series Summary</td></tr>
        <tr><td>
        <localAsp:SeriesDetailsView ID="SeriesDetails" runat="server" />
        </td></tr>
    </table>
  </tr>
  
  
  </table>
  
        </ContentTemplate>
    </asp:UpdatePanel>

