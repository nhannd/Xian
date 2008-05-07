<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ServerPartitionTabs" %>

<asp:Panel ID="Panel1" runat="server" BorderWidth="0">
    <aspAjax:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="TabControl">
        <aspAjax:TabPanel ID="TabPanel1" runat="server" HeaderText="Partition Tab" CssClass="CSSTabPanel">
            <HeaderTemplate>
            </HeaderTemplate>
            <ContentTemplate>
            </ContentTemplate>
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
</asp:Panel>
