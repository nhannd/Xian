<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ServerPartitionTabs" %>

    <aspAjax:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="TabControl" >
        <aspAjax:TabPanel ID="PartitionTabPanel" runat="server">
            <HeaderTemplate>
            Add Partition
            </HeaderTemplate>
            <ContentTemplate>
            
            <asp:Panel runat="server" CssClass="AddPartitionMessage">
                There are currently no partitions setup on the ImageServer. <asp:LinkButton runat="server" PostBackUrl="~/Pages/Admin/Configure/ServerPartitions/Default.aspx" CssClass="AddPartitionLink">Add New Partition</asp:LinkButton>
            </asp:Panel>
            
            </ContentTemplate>            
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
    
