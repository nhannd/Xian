<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Common/Pages/MainContentSection.Master" Codebehind="StudyDetailsPage.aspx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPage" %>

<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsPanel.ascx" TagName="StudyDetailsPanel" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <table width="100%"><tr><td align="right" style="padding-top: 17px;"><asp:LinkButton ID="LinkButton1" runat="server" Text="Close" Font-Size="18px" Font-Bold="true" ForeColor="white" Font-Underline="false" OnClientClick="javascript: window.close(); return false;" /></td></tr></table>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContentSection" runat="server">
            <asp:UpdatePanel runat="server" ID="updatepanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <localAsp:StudyDetailsPanel ID="StudyDetailsPanel" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>
