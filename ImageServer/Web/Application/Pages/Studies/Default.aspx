<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false" Codebehind="Default.aspx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Default" %>

<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetails/Controls/AddAuthorityGroupsDialog.ascx" TagName="AddAuthorityGroupsDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,Studies%>" /></asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" Visible="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    

    <localAsp:DeleteStudyConfirmDialog ID="DeleteStudyConfirmDialog" runat="server"/>
    <localAsp:AddAuthorityGroupsDialog ID="AddAuthorityGroupsDialog" runat="server"/>
</asp:Content>
