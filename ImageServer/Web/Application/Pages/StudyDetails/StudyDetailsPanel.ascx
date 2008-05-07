<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsPanel" %>

<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="StudyDetailsPanelContainer">
            <asp:Panel  runat="server" ID="MessagePanel" 
                            style="margin-bottom:10px; border-right: #dcdcdc 1px solid; border-top: #dcdcdc 1px solid; border-left: #dcdcdc 1px solid; border-bottom: #dcdcdc 1px solid; background-color: #ffffcc; text-align:center; padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px;">
                 <asp:Label ID="ConfirmationMessage" runat="Server" Text="" />
            </asp:Panel>
            
            <localAsp:PatientSummaryPanel ID="PatientSummaryPanel" runat="server"></localAsp:PatientSummaryPanel>
            <br />             
            <ccAsp:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="STUDY" HeadingCSS="CSSStudyHeading" ContentAreaCSS="CSSSectionContent"
                Width="100%" CssClass="CSSSection">
                <SectionContentTemplate>
                    <asp:Panel ID="Panel6" runat="server" CssClass="CSSToolbarPanelContainer">
                    <asp:Panel ID="Panel7" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                            <asp:Panel ID="Panel8" runat="server" CssClass="CSSToolbarContent">
                                <ccUI:ToolbarButton ID="DeleteToolbarButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                                    DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this study"
                                    OnClick="DeleteToolbarButton_Click" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    <localAsp:StudyDetailsView ID="StudyDetailsView" runat="server" Width="50%"></localAsp:StudyDetailsView>
                </SectionContentTemplate>
            </ccAsp:SectionPanel>
            
            <br />
            <ccAsp:SectionPanel ID="SeriesSectionPanel" runat="server" HeadingText="SERIES" HeadingCSS="CSSStudyHeading"
                Width="100%" CssClass="CSSSection">
                <SectionContentTemplate>
                    <asp:Panel ID="Panel1" runat="server" CssClass="CSSToolbarPanelContainer">
                    <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarContent">
                                <ccUI:ToolbarButton ID="OpenSeriesButton" runat="server" EnabledImageURL="~/images/icons/ViewDetailsEnabled.png"
                                    DisabledImageURL="~/images/icons/ViewDetailsDisabled.png" AlternateText="Open" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    
                    <localAsp:SeriesGridView ID="SeriesGridView" runat="server"></localAsp:SeriesGridView>
                </SectionContentTemplate>
            </ccAsp:SectionPanel>
            
            
        </asp:Panel>
                 
        <ccAsp:ConfirmationDialog ID="ConfirmDialog" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
