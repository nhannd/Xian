<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ServerPartitionTabs" %>

    <asp:Panel runat="server" ID="PartitionPanel" CssClass="PartitionPanel">
    <asp:Label ID="Label1" runat="server" Text="Partitions" CssClass="SearchTextBoxLabel" EnableViewState="False" style="padding-left: 5px;"/><br />    
    <aspAjax:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="PartitionTabControl" >
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
    </asp:Panel>
    
