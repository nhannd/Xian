<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPanel" %>

<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="MainContentTitle">Study Details</td><td class="ButtonPanel" style="padding-right: 5px;"><asp:Button runat="server" ID="EditStudyButton" Text="Edit" CssClass="ButtonStyle" /><asp:Button runat="server" ID="DeleteStudyButton" Text="Delete" OnClick="DeleteStudyButton_Click" CssClass="ButtonStyle" /></td>
            </tr>
            <tr>
                <td colspan="2" class="PatientInfo">
                    <table width="100%" cellpadding="0" cellspacing="0" style="background-color: #eeeeee">
                        <tr><td>
                            <asp:Panel CssClass="StudyDetailsErrorMessage" runat="server" ID="MessagePanel" ><asp:Label ID="ConfirmationMessage" runat="Server" Text="" /></asp:Panel>
                        </td></tr>
                        <tr>
                            <td>
                                <localAsp:PatientSummaryPanel ID="PatientSummaryPanel" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="StudyDetailsContent">                  
                    <localAsp:StudyDetailsView ID="StudyDetailsView" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="SeriesContent">
                    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                        <tr><td class="MainContentSubTitle" >Series</td><td class="ButtonPanel"><asp:Button ID="ViewSeriesButton" runat="server" Text="View Details" CssClass="ButtonStyle" width="85px"/></td></tr>
                        <tr><td colspan="2"><localAsp:SeriesGridView ID="SeriesGridView" runat="server" /></td></tr>
                    </table>
                </td>
            </tr>
            </table>

  </div>
    
               
        <ccAsp:ConfirmationDialog ID="ConfirmDialog" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
