<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/GlobalMasterPage.master" Codebehind="StudyDetailsPage.aspx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPage" %>

<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="uc2" %>
<%@ Register Src="StudyDetailsPanel.ascx" TagName="StudyDetailsPanel" TagPrefix="uc1" %>

<asp:Content runat="server" ID="MainMenuContent" ContentPlaceHolderID="MainMenuPlaceHolder">
    <table width="100%"><tr><td align="right" style="padding-top: 17px;"><asp:LinkButton ID="LinkButton1" runat="server" Text="Close" Font-Size="18px" Font-Bold="true" ForeColor="white" Font-Underline="false" OnClientClick="javascript: window.close(); return false;" /></td></tr></table>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
            <asp:UpdatePanel runat="server" ID="updatepanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel runat="server" Width="100%">
                        <uc1:StudyDetailsPanel ID="StudyDetailsPanel" runat="server" />
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>
