<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ServerPartitionTabs" %>

<asp:Panel ID="Panel1" runat="server" BorderWidth="0">
    <ajaxToolkit:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="TabControl">
        <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="Partition Tab" CssClass="CSSTabPanel">
            <HeaderTemplate>
            </HeaderTemplate>
            <ContentTemplate>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Panel>
