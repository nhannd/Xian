<%@ Page Language="C#" MasterPageFile="~/Pages/Common/MainContentSection.master" AutoEventWireup="true" 
    EnableEventValidation="false" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.Default" 
    Title="Move Series | ClearCanvas ImageServer" %>

<%@ Register Src="MovePanel.ascx" TagName="MovePanel" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="ContentTitle" ContentPlaceHolderID="TitlePlaceHolder">Move Series</asp:Content>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server" Width="100%" ><asp:TableRow><asp:TableCell HorizontalAlign="right" style="padding-top: 12px;"><asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" OnClientClick="javascript: window.close(); return false;" /></asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentSectionPlaceHolder" runat="server">
    <localAsp:MovePanel ID="MoveSeriesPanel" runat="server"/>
</asp:Content>
