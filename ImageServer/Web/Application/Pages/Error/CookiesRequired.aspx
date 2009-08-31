<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CookiesRequired.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.CookiesRequired" %>

<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="Label1" Text="Cookies are currently disabled on your browser." runat="server" />

</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
		ClearCanvas ImageServer requires your browser to accept Cookies. 
		Please enable Cookies on your browser and click <a href="../../Default.aspx" class="ErrorLink">here</a> to try again.
</asp:Content>