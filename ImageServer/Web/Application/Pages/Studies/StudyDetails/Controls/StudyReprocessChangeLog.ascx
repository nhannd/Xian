<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyReprocessChangeLog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.StudyReprocessChangeLogControl" %>

<asp:Panel runat="server" ID="DetailsPanel" CssClass="HistoryDetailContainer">
    <% if (!String.IsNullOrEmpty(ChangeLog.User)) {%>
    By <%=ChangeLog.User %> <br />
    <% } %>
    <%= ChangeLog.Reason %>
</asp:Panel>