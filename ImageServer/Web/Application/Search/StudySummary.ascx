<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummary.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudySummary" %>
<asp:Panel ID="DetailsPanel" runat="server">
    <asp:Label ID="PatientName" runat="server" Text="" EnableViewState="False" Width="24%"/>
    <asp:Label ID="PatientId" runat="server" Text="" EnableViewState="False" Width="12%"/>
    <asp:Label ID="StudyDate" runat="server" Text="" EnableViewState="False" Width="12%" />
    <asp:Label ID="AccessionNumber" runat="server" Text="" EnableViewState="False" Width="19%"/>
    <asp:Label ID="StudyDescription" runat="server" Text="" EnableViewState="False" Width="23%"/>
</asp:Panel>
