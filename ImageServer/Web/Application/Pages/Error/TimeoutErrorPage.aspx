<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeoutErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.TimeoutErrorPage" %>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Import namespace="System.Threading"%>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" Text="ImageServer session timed out." runat="server" />
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        This message is being displayed because there has been no activity in the past <%= Math.Round(SessionManager.SessionTimeout.TotalMinutes) %> minute(s). For security purposes, 
        your ImageServer session has been closed. Please <asp:LinkButton runat="server" PostBackUrl="~/Pages/Login/Default.aspx" >login</asp:LinkButton> again to continue using ClearCanvas ImageServer.
         <br/><br/>
         For more information, please refer to one of the forums below.
    </asp:Label>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td class="UserEscapeCell" style="width: 50%"><asp:LinkButton ID="LogoutButton" runat="server" CssClass="UserEscapeLink" OnClick="Login_Click">Login</asp:LinkButton></td><td style="width: 50%"><a href="javascript:window.close()" class="UserEscapeLink">Close</a></td></tr></table>
</asp:Content>