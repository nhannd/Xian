<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.WorkQueueSummaryPanel" %>
<asp:Panel ID="DetailsPanel" runat="server">
    <asp:Label ID="PatientID" runat="server" EnableViewState="False" Width="15%" />
    <asp:Label ID="PatientsName" runat="server" EnableViewState="False" Width="30%" />
    <asp:Label ID="WorkQueueType" runat="server" EnableViewState="False" Width="15%" />
    <asp:Label ID="WorkQueueStatus" runat="server" EnableViewState="False" Width="15%" />
    <asp:Label ID="ScheduledTime" runat="server" EnableViewState="False" />
</asp:Panel>
