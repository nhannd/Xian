<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyDetailsDialogPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialogPanel" %>

<%@ Register Src="DeletedStudyDetailsDialogGeneralPanel.ascx" TagName="GeneralInfoPanel" TagPrefix="localAsp" %>
<%@ Register Src="DeletedStudyArchiveInfoPanel.ascx" TagName="ArchiveInfoPanel" TagPrefix="localAsp" %>
    
<asp:Panel ID="Panel3" runat="server">
    <aspAjax:TabContainer ID="TabContainer" runat="server" ActiveTabIndex="0" CssClass="DialogTabControl">
        <aspAjax:TabPanel ID="StudyInfoTabPanel" runat="server" HeaderText="Study Information" CssClass="DialogTabControl">
            <ContentTemplate>
                <localAsp:GeneralInfoPanel runat="server" ID="GeneralInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
        
        <aspAjax:TabPanel ID="ArchiveInfoTabPanel" runat="server" HeaderText="Archive" CssClass="DialogTabControl">
            <ContentTemplate>
             <localAsp:ArchiveInfoPanel runat="server" ID="ArchiveInfoPanel" />
            </ContentTemplate>
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
    
</asp:Panel>