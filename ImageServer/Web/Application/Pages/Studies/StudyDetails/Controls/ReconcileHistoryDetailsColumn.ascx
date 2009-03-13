<%@ Import namespace="ClearCanvas.ImageServer.Common.Data"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common.CommandProcessor"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReconcileHistoryDetailsColumn.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.ReconcileHistoryDetailsColumn" %>

<asp:Panel runat="server" CssClass="HistoryDetailContainer">
<asp:Table ID="Table1" runat="server"  BorderColor="gray" BorderWidth="0px" CellPadding="0" CellSpacing="0">
<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top" Width="120px">Description</asp:TableCell>
<asp:TableCell ><%# ReconcileHistory.Description %></asp:TableCell>
</asp:TableRow>
<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top">Action</asp:TableCell>
<asp:TableCell ><%# ReconcileHistory.Action %> (<%# ReconcileHistory.Automatic? "Auto":"Manual" %>)</asp:TableCell>
</asp:TableRow>

<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top">Reconciled Images</asp:TableCell>
<asp:TableCell >
<% foreach(ImageSetField field in ReconcileHistory.ImageSetData.Fields) { %>
        <%= HtmlUtility.Encode(field.DicomTag.Name)%> = <%= field.Value %><br />
<%} %>
</asp:TableCell>
</asp:TableRow>

<asp:TableRow BorderWidth="1px">
<asp:TableCell BorderWidth="0px" VerticalAlign="top">Modifications:</asp:TableCell>
<asp:TableCell BorderWidth="0px">
<% if (ReconcileHistory.Commands == null || ReconcileHistory.Commands.Count == 0)
 {%>
    N/A    
<%} else {
        foreach (BaseImageLevelUpdateCommand cmd in ReconcileHistory.Commands)
        {%>
            <%= HtmlUtility.Encode(cmd.ToString()) %><br />
        <%}%>
<%}%>
</asp:TableCell>
</asp:TableRow></asp:Table>
</asp:Panel>

