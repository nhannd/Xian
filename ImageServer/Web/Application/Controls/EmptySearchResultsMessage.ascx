<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptySearchResultsMessage.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.EmptySearchResultsMessage" %>

<asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
    <asp:Label runat="server" ID="ResultsMessage" Text = "No items found using the provided criteria." />
</asp:Panel>