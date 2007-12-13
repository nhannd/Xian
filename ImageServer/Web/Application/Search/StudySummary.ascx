<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummary.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudySummary" %>
<asp:Panel ID="DetailsPanel" runat="server" >
    <asp:Label ID="PatientName" runat="server" Text="" EnableViewState="False"
        Width="24%" CssClass="AccordianCell" />
    <asp:Label ID="PatientId" runat="server" Text="" EnableViewState="False"
        Width="12%" CssClass="AccordianCell" />
    <asp:Label ID="StudyDate" runat="server" Text="" EnableViewState="False"
        Width="11%" CssClass="AccordianCell" />
    <asp:Label ID="AccessionNumber" runat="server" Text="" EnableViewState="False"
        Width="19%" CssClass="AccordianCell" />
    <asp:Label ID="StudyDescription" runat="server" Text="" EnableViewState="False"
        Width="24%" CssClass="AccordianCell" />
</asp:Panel>
