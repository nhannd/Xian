<%-- License
// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LicenseErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.LicenseErrorPage" %>
<%@ Import namespace="System.Threading"%>

<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" runat="server">
	        <%= ErrorMessages.LicenseError %>
	    </asp:label>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
   <asp:Label ID = "DescriptionLabel" runat="server">
        <%= ClearCanvas.ImageServer.Web.Common.Utilities.HtmlUtility.Encode(ErrorMessages.LicenseErrorLongDescription)%>
    </asp:Label>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table runat="server" width="100%" class="UserEscapeTable">
        <tr>
            <td class="UserEscapeCell"><a href="javascript:window.location.reload()" class="UserEscapeLink"><%= Labels.Refresh %></a></td>
        </tr>
    </table>
</asp:Content>