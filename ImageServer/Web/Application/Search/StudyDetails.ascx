<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetails.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudyDetails" %>

<asp:Panel ID="DetailsPanel" runat="server" CssClass="toolBarPanel">
<asp:Label runat="server" ID="StudyLabel" CssClass="sectionLabel" Text="Study:"></asp:Label>
    <hr class="sectionDivLine" />

<asp:Table ID="DetailsTable" runat="server"></asp:Table>
</asp:Panel>

