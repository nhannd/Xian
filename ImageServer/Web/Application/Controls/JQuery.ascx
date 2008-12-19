<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JQuery.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.JQuery" %>

<%if(MultiSelect) { %>
    <link href="<%=ResolveUrl("~/App_Themes/" + ImageServerConstants.Theme + "/StyleSheets/Controls/jqueryMultiSelect.css") %>" rel="stylesheet" type="text/css" />
<%} %>