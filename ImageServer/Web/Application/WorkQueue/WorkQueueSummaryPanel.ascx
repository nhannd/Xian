<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.WorkQueueSummaryPanel" %>
<asp:Panel ID="DetailsPanel" runat="server">
    <asp:Label ID="PatientID" runat="server" EnableViewState="False" Width="100px" CssClass="AccordianCell" />
    <asp:Label ID="PatientsName" runat="server" EnableViewState="False" Width="230px"
        CssClass="AccordianCell" />
    <asp:Label ID="WorkQueueType" runat="server" EnableViewState="False" Width="100px"
        CssClass="AccordianCell" />
    <asp:Label ID="WorkQueueStatus" runat="server" EnableViewState="False" Width="80px"
        CssClass="AccordianCell" />
    <asp:Label ID="ScheduledTime" runat="server" EnableViewState="False" Width="150px"
        CssClass="AccordianCell" />
</asp:Panel>
