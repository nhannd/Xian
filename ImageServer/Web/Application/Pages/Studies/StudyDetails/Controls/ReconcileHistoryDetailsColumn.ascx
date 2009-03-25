<%@ Import namespace="ClearCanvas.ImageServer.Common.Data"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common.CommandProcessor"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReconcileHistoryDetailsColumn.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.ReconcileHistoryDetailsColumn" %>

<asp:Panel runat="server" CssClass="HistoryDetailContainer">
<asp:Table ID="Table1" runat="server"  BorderColor="gray" BorderWidth="0px" CellPadding="0" CellSpacing="0">
<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top"  Width="140px">Reconciled Images</asp:TableCell>
<asp:TableCell >
    <%  if (ReconcileHistory.ImageSetData.Fields == null || ReconcileHistory.ImageSetData.Fields.Length==0)
        { %>
            N/A
    <%  }
        else foreach(ImageSetField field in ReconcileHistory.ImageSetData.Fields) { %>
            <%= HtmlUtility.Encode(field.DicomTag.Name)%> = <%= field.Value %><br />
    <%} %>
</asp:TableCell>
</asp:TableRow>

<asp:TableRow BorderWidth="1px">
<asp:TableCell VerticalAlign="top">Action (<%# ReconcileHistory.Automatic ? "Auto" : "<b>Manual</b>"%>)</asp:TableCell>
<asp:TableCell >
    <%# HtmlUtility.GetEnumInfo(ReconcileHistory.Action).LongDescription%></asp:TableCell>
</asp:TableRow>


<asp:TableRow BorderWidth="1px">
<asp:TableCell BorderWidth="0px" VerticalAlign="top">Changes to study:</asp:TableCell>
<asp:TableCell BorderWidth="0px">
<% if (ReconcileHistory.Commands == null || ReconcileHistory.Commands.Count == 0)
 {%>
    None 
<%} else {
        foreach (BaseImageLevelUpdateCommand cmd in ReconcileHistory.Commands)
        {%>
            <%= HtmlUtility.Encode(cmd.ToString()) %><br />
        <%}%>
<%}%>
</asp:TableCell>
</asp:TableRow></asp:Table>
</asp:Panel>

