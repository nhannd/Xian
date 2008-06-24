<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.ErrorPage" %>

<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    Javascript is currently disabled on your browser.
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">
		ClearCanvas ImageServer requires Javascript in order to run. 
		Please enable Javascript on your browser and click <a href="../../Default.aspx" class="ErrorLink">here</a> to try again.
</asp:Content>