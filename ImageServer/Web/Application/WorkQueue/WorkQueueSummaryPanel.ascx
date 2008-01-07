<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.WorkQueueSummaryPanel" %>
<asp:Panel ID="DetailsPanel" runat="server">
    <asp:Label ID="PatientID" runat="server" EnableViewState="False" Width="100px" CssClass="CSSAccordianCell" />
    <asp:Label ID="PatientsName" runat="server" EnableViewState="False" Width="230px"
        CssClass="CSSAccordianCell" />
    <asp:Label ID="WorkQueueType" runat="server" EnableViewState="False" Width="100px"
        CssClass="CSSAccordianCell" />
    <asp:Label ID="WorkQueueStatus" runat="server" EnableViewState="False" Width="80px"
        CssClass="CSSAccordianCell" />
    <asp:Label ID="ScheduledTime" runat="server" EnableViewState="False" Width="180px"
        CssClass="CSSAccordianCell" />
</asp:Panel>
