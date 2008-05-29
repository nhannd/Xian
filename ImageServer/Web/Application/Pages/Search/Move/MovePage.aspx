<%@ Page Language="C#" MasterPageFile="~/Common/Pages/MainContentSection.master" AutoEventWireup="true" 
    EnableEventValidation="false" CodeBehind="MovePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.Move.MovePage" 
    Title="Move Studies" %>

<%@ Register Src="MovePanel.ascx" TagName="MovePanel" TagPrefix="localAsp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <localAsp:MovePanel ID="Move" runat="server"/>
</asp:Content>
