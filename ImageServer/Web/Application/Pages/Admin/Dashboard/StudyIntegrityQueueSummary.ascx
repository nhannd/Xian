<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyIntegrityQueueSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.StudyIntegrityQueueSummary" %>

<table cellpadding="2" width="100%">
    <tr><td style="color: #205F87;">Duplicates</td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="DuplicateLink"><%=Duplicates %></asp:LinkButton></td></tr>
    <tr><td style="color: #205F87;">Inconsistent Data</td><td align="right" style="padding-right: 15px;"><asp:LinkButton runat="server" ID="InconsistentDataLink"><%=InconsistentData %></asp:LinkButton></td></tr>
    <tr><td align="right" style="padding-right: 15px; color: #205F87;"><b>Total</b></td><td align="right" style="padding-right: 15px;"><b><asp:LinkButton runat="server" ID="TotalLinkButton"><%= Duplicates + InconsistentData %></asp:LinkButton></b></td></tr>
</table>