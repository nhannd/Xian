<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsPanel" %>

<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="uc3" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="uc1" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="uc2" %>

<asp:Panel ID="Panel1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="SeriesDetailsPanelContainer">
                <uc2:PatientSummaryPanel ID="PatientSummaryPanel1" runat="server" />
                <br />
                <ccAsp:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="STUDY" HeadingCSS="CSSStudyHeading"
                    Width="100%" CssClass="CSSSection">
                    <SectionContentTemplate>
                        <uc3:StudySummaryPanel ID="StudySummaryPanel1" runat="server" />
                    </SectionContentTemplate>
                </ccAsp:SectionPanel>
                <br />
                <ccAsp:SectionPanel ID="SeriesSectionPanel" runat="server" HeadingText="SERIES" HeadingCSS="CSSStudyHeading"
                    Width="100%" CssClass="CSSSection">
                    <SectionContentTemplate>
                    <uc1:SeriesDetailsView ID="SeriesDetailsView1" runat="server"></uc1:SeriesDetailsView>
                    </SectionContentTemplate>
                </ccAsp:SectionPanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
