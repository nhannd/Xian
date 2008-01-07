<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetails.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudyDetails" %>
<asp:Panel ID="DetailsPanel" runat="server" CssClass="CSSDetailsPanel">
    <asp:Label runat="server" ID="StudyLabel" CssClass="CSSSectionLabel" Text="Study:"></asp:Label>
    <hr class="sectionDivLine" />
    <asp:Table ID="DetailsTable" runat="server">
    </asp:Table>
</asp:Panel>
