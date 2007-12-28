<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ServerPartitionTabs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Panel ID="Panel1" runat="server" BorderWidth="0">
    <ajaxToolkit:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="visoft__tab_xpie7">
        <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="Partition Tab">
            <HeaderTemplate>
            </HeaderTemplate>
            <ContentTemplate>
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</asp:Panel>
