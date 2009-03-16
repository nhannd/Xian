<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorizationErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.AuthorizationErrorPage" %>
<%@ Import namespace="System.Threading"%>

<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" Text="Something happened that the ImageServer was unprepared for." runat="server" />
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        This message is being displayed because the ClearCanvas ImageServer encountered a 
        situation that was unexpected. The resulting error has been recorded for future analysis.
         <br/><br/>
         If you would like to report the error, please post to one of the forums listed below and 
         provide any information you think will be helpful in handling this situation in the future. 
         <asp:panel ID="StackTraceMessage" runat="server" Visible="false">To include the ImageServer's error message, <a class="ErrorLink" href="javascript:toggleLayer('StackTrace');">click here</a>.</asp:panel>
    </asp:Label>
    <div id="StackTrace" style="margin-top: 15px" visible="false"><asp:TextBox runat="server" ID="StackTraceTextBox" Visible="false" Rows="5" Columns="57" TextMode="MultiLine" ReadOnly="true"></asp:TextBox></div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td class="UserEscapeCell"><a href="javascript:history.back()" class="UserEscapeLink">Back</a></td><td class="UserEscapeCell"><asp:LinkButton ID="LogoutButton" runat="server" CssClass="UserEscapeLink" OnClick="Logout_Click">Logout</asp:LinkButton></td><td><a href="javascript:window.close()" class="UserEscapeLink">Close</a></td></tr></table>
</asp:Content>