<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog.Default" Title="Application Log" %>

<%@ Register Src="ApplicationLogPanel.ascx" TagName="ApplicationLogPanel" TagPrefix="localAsp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">
Application Log
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <localAsp:ApplicationLogPanel ID="ApplicationLogPanel" runat="server"/>
            </asp:Panel>
            
        </ContentTemplate>
      
    </asp:UpdatePanel>

</asp:Content>
