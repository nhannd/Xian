<%-- License
// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebViewerTimeoutErrorPage.aspx.cs" MasterPageFile="WebViewerErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.WebViewerTimeoutErrorPage" %>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Import namespace="System.Threading"%>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" Text="ImageServer session timed out." runat="server" />
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        This message is being displayed because there has been no activity in the past <%= Math.Round(SessionManager.SessionTimeout.TotalMinutes) %> minute(s). For security purposes, 
        your ImageServer session has been closed.
    </asp:Label>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td style="height: 30px; text-align: center;"><a href="javascript:window.close()" class="UserEscapeLink">Close</a></td></tr></table>
</asp:Content>