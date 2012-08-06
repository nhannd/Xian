<%--  License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Default" %>

<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>
<%@ Register Src="StudyDetails/Controls/AddAuthorityGroupsDialog.ascx" TagName="AddAuthorityGroupsDialog" TagPrefix="localAsp" %>
<%@ Register Src="SearchPanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">
    <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,Studies%>" /></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">    
    <asp:UpdatePanel ID="PageContent" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <ccAsp:ServerPartitionSelector runat="server" id="ServerPartitionSelector" visible="true" />
        <localAsp:SearchPanel runat="server" id="SearchPanel" visible="true" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DialogsPlaceHolder" runat="server">
    <localAsp:DeleteStudyConfirmDialog id="DeleteStudyConfirmDialog" runat="server" />
    <localAsp:AddAuthorityGroupsDialog id="AddAuthorityGroupsDialog" runat="server" />
</asp:Content>
