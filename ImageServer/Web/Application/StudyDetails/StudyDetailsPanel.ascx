<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Common/SectionPanel.ascx" TagName="SectionPanel" TagPrefix="clearcanvas" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="clearcanvas" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="clearcanvas" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="clearcanvas" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    Assembly="ClearCanvas.ImageServer.Web.Common" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="StudyDetailsPanelContainer">
            <asp:Panel  runat="server" ID="MessagePanel" 
                            style="margin-bottom:10px; border-right: #dcdcdc 1px solid; border-top: #dcdcdc 1px solid; border-left: #dcdcdc 1px solid; border-bottom: #dcdcdc 1px solid; background-color: #ffffcc; text-align:center; padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px;">
                 <asp:Label ID="ConfirmationMessage" runat="Server" Text="" />
            </asp:Panel>
            
            <clearcanvas:PatientSummaryPanel ID="PatientSummaryPanel" runat="server"></clearcanvas:PatientSummaryPanel>
            <br />             
            <clearcanvas:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="STUDY" HeadingCSS="CSSStudyHeading" ContentAreaCSS="CSSSectionContent"
                Width="100%" CssClass="CSSSection">
                <SectionContentTemplate>
                    <asp:Panel ID="Panel6" runat="server" CssClass="CSSToolbarPanelContainer">
                    <asp:Panel ID="Panel7" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                            <asp:Panel ID="Panel8" runat="server" CssClass="CSSToolbarContent">
                                <clearcanvas:ToolbarButton ID="DeleteToolbarButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                                    DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this study"
                                    OnClick="DeleteToolbarButton_Click" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    <clearcanvas:StudyDetailsView ID="StudyDetailsView" runat="server" Width="50%"></clearcanvas:StudyDetailsView>
                </SectionContentTemplate>
            </clearcanvas:SectionPanel>
            
            <br />
            <clearcanvas:SectionPanel ID="SeriesSectionPanel" runat="server" HeadingText="SERIES" HeadingCSS="CSSStudyHeading"
                Width="100%" CssClass="CSSSection">
                <SectionContentTemplate>
                    <asp:Panel ID="Panel1" runat="server" CssClass="CSSToolbarPanelContainer">
                    <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarContent">
                                <clearcanvas:ToolbarButton ID="OpenSeriesButton" runat="server" EnabledImageURL="~/images/icons/OpenEnabled.png"
                                    DisabledImageURL="~/images/icons/OpenDisabled.png" AlternateText="Open" />
                            </asp:Panel>
                        </asp:Panel>
                    </asp:Panel>
                    
                    <clearcanvas:SeriesGridView ID="SeriesGridView" runat="server"></clearcanvas:SeriesGridView>
                </SectionContentTemplate>
            </clearcanvas:SectionPanel>
            
            <%--
            
            <asp:Panel ID="Panel1" runat="server">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <strong>STUDY</strong>
                        </td>
                        <td align="center">
                            
                        </td>
                    </tr>
                </table>
                <hr />
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelContainer">
                <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                    <asp:Panel ID="Panel5" runat="server" CssClass="CSSToolbarContent">
                        <clearcanvas:ToolbarButton ID="DeleteToolbarButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                            DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this study"
                            OnClick="DeleteToolbarButton_Click" />
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="StudyDetailsViewPanel">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr valign="top">
                        <td width="50%">
                            <clearcanvas:StudyDetailsView ID="StudyDetailsView1" runat="server"></clearcanvas:StudyDetailsView>
                        </td>
                        <td width="50%" valign="top" align="right">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
            <br />
            <asp:Panel ID="Panel2" runat="server">
                <strong>SERIES</strong>
            </asp:Panel>
            <clearcanvas:SeriesGridView ID="SeriesGridView1" runat="server"></clearcanvas:SeriesGridView>
            --%>
            
            
        </asp:Panel>
                 
        <clearcanvas:ConfirmationDialog ID="ConfirmDialog1" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
