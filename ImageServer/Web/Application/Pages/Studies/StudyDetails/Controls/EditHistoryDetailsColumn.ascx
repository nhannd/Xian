<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common.CommandProcessor"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistoryDetailsColumn.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditHistoryDetailsColumn" %>

<asp:Panel ID="Panel1" runat="server" CssClass="HistoryDetailContainer">
<asp:Table ID="Table1" runat="server" CssClass="ReconcileDetails" BorderColor="gray" BorderWidth="0px" CellPadding="0" CellSpacing="0">
<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top" Width="140px">Action</asp:TableCell>
<asp:TableCell ><%# HtmlUtility.GetEnumInfo(EditHistory.EditType).LongDescription%></asp:TableCell>
</asp:TableRow>
<asp:TableRow BorderWidth="1px">
<asp:TableCell BorderWidth="0px" VerticalAlign="top">Changes</asp:TableCell>
<asp:TableCell BorderWidth="0px">
<% foreach(BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
 {%>
    <%= HtmlUtility.Encode(cmd.ToString()) %><br />
<%
 }%>
</asp:TableCell>
</asp:TableRow></asp:Table>
</asp:Panel>

