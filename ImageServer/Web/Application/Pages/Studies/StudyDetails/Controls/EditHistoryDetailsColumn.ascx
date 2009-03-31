<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common.CommandProcessor"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHistoryDetailsColumn.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.EditHistoryDetailsColumn" %>

<asp:Panel runat="server" ID="SummaryPanel">
    <asp:Label runat="server" ID="Label1">
    </asp:Label>
</asp:Panel>

<asp:Panel runat="server" ID="DetailsPanel" CssClass="HistoryDetailContainer">
    <asp:Table ID="Table2" runat="server" BorderColor="gray" BorderWidth="0px" CellPadding="0"
        CellSpacing="0" CssClass="HistoryDetailTable">
            <asp:TableRow BorderWidth="0px">
            <asp:TableCell BorderWidth="0px">
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr>
                            <td class="HistoryDetailsHeading">
                                <span style="margin-left:5px">
                                    Action : <%= HtmlUtility.GetEnumInfo(EditHistory.EditType).LongDescription %> 
                                </span>
                            </td></tr>
                        <tr><td style="border:none">
                        <div style="margin-left:2px;">
                            <% if (EditHistory.UpdateCommands == null || EditHistory.UpdateCommands.Count == 0)
                               {%>
                                <pre  style="padding-left:10px">Study was not changed.</pre>
                            <%}
                              else
                              {
                                  foreach(BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
                                  { %>
                                        <pre  style="padding-left:15px"><%= HtmlUtility.Encode(cmd.ToString()) %></pre>
                                <%}%>
                            <%}%>
                         </div>
                        </td></tr>
                    </table>
                </div>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<aspAjax:CollapsiblePanelExtender ID="cpe" runat="Server" TargetControlID="DetailsPanel"
    AutoExpand="false" CollapseControlID="SummaryPanel" ExpandControlID="SummaryPanel"
    Collapsed="true" CollapsedText='<%# String.Format("{0} (Click for details)", HtmlUtility.GetEnumInfo(EditHistory.EditType).LongDescription)%>'
    ExpandedText=" " TextLabelID="Label1">
</aspAjax:CollapsiblePanelExtender>

<%--
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


--%>