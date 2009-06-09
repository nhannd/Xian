<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyDetailsPanel" %>

<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsTabs.ascx" TagName="StudyDetailsTabs" TagPrefix="localAsp" %>
<%@ Register Src="StudyStateAlertPanel.ascx" TagName="StudyStateAlertPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="conditional">
    <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="MainContentTitle">Study Details</td><td class="MainContentTitleButtonPanel">
                    <ccUI:ToolbarButton runat="server" ID="EditStudyButton" SkinID="EditButton" OnClick="EditStudyButton_Click" />
                    <ccUI:ToolbarButton runat="server" ID="DeleteStudyButton" SkinID="DeleteButton" OnClick="DeleteStudyButton_Click" />
                    <ccUI:ToolbarButton runat="server" ID="ReprocessStudyButton" SkinID="ReprocessButton" OnClick="ReprocessButton_Click" />
                </td>
            </tr>
            <tr>
                <td class="PatientInfo" colspan="2">
                    <table width="100%" cellpadding="0" cellspacing="0" class="PatientSummaryTable">
                        <tr><td>
                            <localAsp:StudyStateAlertPanel runat="server" ID="StudyStateAlertPanel" />
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
                <td class="Spacer" colspan="2"><asp:Image runat="server" SkinID="Spacer" Height="3px"/></td>
            </tr>
            <tr>
                <td colspan="2">                  
                    <localAsp:StudyDetailsTabs ID="StudyDetailsTabs" runat="server" />
                </td>
            </tr>
            </table>
    </div>                     
    </ContentTemplate>
</asp:UpdatePanel>


