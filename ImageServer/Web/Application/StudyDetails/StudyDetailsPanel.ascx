<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Common/SectionPanel.ascx" TagName="SectionPanel" TagPrefix="uc4" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="uc3" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="uc2" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="clearcanvas" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    Assembly="ClearCanvas.ImageServer.Web.Common" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="cc2" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="StudyDetailsPanelContainer">
            <asp:Panel  runat="server" ID="MessagePanel" 
                            style="margin-bottom:10px; border-right: #dcdcdc 1px solid; border-top: #dcdcdc 1px solid; border-left: #dcdcdc 1px solid; border-bottom: #dcdcdc 1px solid; background-color: #ffffcc; text-align:center; padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px;">
                 <asp:Label ID="ConfirmationMessage" runat="Server" Text="" />
            </asp:Panel>
            
            <uc2:PatientSummaryPanel ID="PatientSummaryPanel" runat="server"></uc2:PatientSummaryPanel>
            <br />             
            <uc4:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="STUDY" HeadingCSS="CSSStudyHeading" ContentAreaCSS="CSSSectionContent"
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
            </uc4:SectionPanel>
            
            <br />
            <uc4:SectionPanel ID="SeriesSectionPanel" runat="server" HeadingText="SERIES" HeadingCSS="CSSStudyHeading"
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
                    
                    <uc3:SeriesGridView ID="SeriesGridView" runat="server"></uc3:SeriesGridView>
                </SectionContentTemplate>
            </uc4:SectionPanel>
            
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
            <uc3:SeriesGridView ID="SeriesGridView1" runat="server"></uc3:SeriesGridView>
            --%>
            
            
        </asp:Panel>
                 
        <cc2:ConfirmDialog ID="ConfirmDialog1" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
