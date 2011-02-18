<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionTabs.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ServerPartitionTabs" %>

    <asp:Panel runat="server" ID="PartitionPanel" CssClass="PartitionPanel">
    <asp:Label ID="Label1" runat="server" Text="<%$Resources: Labels,Partitions %>" CssClass="SearchTextBoxLabel" EnableViewState="False" style="padding-left: 5px;"/><br />    
    <aspAjax:TabContainer ID="PartitionTabContainer" runat="server" ActiveTabIndex="0" CssClass="PartitionTabControl" >
        <aspAjax:TabPanel ID="PartitionTabPanel" runat="server">
            <HeaderTemplate>
                <asp:Literal runat="server" Text="<%$Resources: Labels, AddPartition%>"></asp:Literal>
            </HeaderTemplate>
            <ContentTemplate>
            
            <asp:Panel runat="server" CssClass="AddPartitionMessage">
                <asp:Literal runat="server" Text="<%$Resources: SR, NoPartitionSetup %>"></asp:Literal> 
                <asp:LinkButton runat="server" PostBackUrl="~/Pages/Admin/Configure/ServerPartitions/Default.aspx" CssClass="AddPartitionLink" Text="<%$Resources:Labels,AddNewPartition %>"></asp:LinkButton>
            </asp:Panel>
            
            </ContentTemplate>            
        </aspAjax:TabPanel>
    </aspAjax:TabContainer>
    </asp:Panel>
    
