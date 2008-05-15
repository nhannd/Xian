<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false" CodeBehind="MovePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.Move.MovePage" 
    Title="Move Studies" %>


<%@ Register Src="MovePanel.ascx" TagName="MovePanel" TagPrefix="move" %>

<asp:Content ID="LocationName" ContentPlaceHolderID="LocationNamePlaceHolder" runat="server">Move Studies</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                    <move:MovePanel ID="Move" runat="server"/>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel> 
</asp:Content>
