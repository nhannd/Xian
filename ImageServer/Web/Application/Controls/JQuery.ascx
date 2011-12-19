<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JQuery.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.JQuery" %>

<%if(MultiSelect) { %>
    <link href="<%=ResolveUrl("~/App_Themes/" + ImageServerConstants.Theme + "/StyleSheets/Controls/jqueryMultiSelect.css") %>" rel="stylesheet" type="text/css" />
<%} %>