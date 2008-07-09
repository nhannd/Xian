<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyDetailsPanel" %>

<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="MainContentTitle">Study Details</td><td class="MainContentTitleButtonPanel">
                    <ccUI:ToolbarButton runat="server" ID="EditStudyButton" SkinID="EditButton" OnClick="EditStudyButton_Click" />
                    <ccUI:ToolbarButton runat="server" ID="DeleteStudyButton" SkinID="DeleteButton" OnClick="DeleteStudyButton_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2" class="PatientInfo">
                    <table width="100%" cellpadding="0" cellspacing="0" class="PatientSummaryTable">
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
                <td colspan="2">                  
                    <localAsp:StudyDetailsView ID="StudyDetailsView" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                        <tr><td class="MainContentSubTitle" >Series</td><td class="ButtonPanel"><ccUI:ToolbarButton runat="server" ID="ViewSeriesButton" SkinID="ViewDetailsButton" /></td></tr>
                        <tr><td colspan="2"><localAsp:SeriesGridView ID="SeriesGridView" runat="server" /></td></tr>
                    </table>
                </td>
            </tr>
            </table>

  </div>
    
               
        <ccAsp:MessageBox ID="ConfirmDialog" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
