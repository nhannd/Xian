<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesDetails.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SeriesDetails" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Panel runat="server" ID="DetailsPanel" CssClass="CSSDetailsPanel">
    <asp:Label runat="server" ID="SeriesLabel" CssClass="CSSSectionLabel" Text="Series:"></asp:Label>
    <hr class="sectionDivLine" />
    <asp:Table ID="DetailsTable" runat="server" Width="100%">
    </asp:Table>
</asp:Panel>
