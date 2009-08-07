<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDeleteChangeLog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesDeleteChangeLog" %>

<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common.CommandProcessor" %>

<asp:Panel runat="server" ID="SummaryPanel">
    <asp:Label runat="server" ID="Label1">
    </asp:Label>
</asp:Panel>
<asp:Panel runat="server" ID="DetailsPanel" CssClass="HistoryDetailContainer">
    <asp:Table ID="Table1" runat="server" BorderColor="gray" BorderWidth="0px" CellPadding="0"
        CellSpacing="0" CssClass="HistoryDetailTable">
        <asp:TableRow BorderWidth="0px">
            <asp:TableCell BorderWidth="0px" VerticalAlign="top">
                <div class="HistoryDetailsSectionPanel">
                    <table border="0" width="100%">
                        <tr>
                            <td class="HistoryDetailsHeading">
                                <span style="margin-left:5px">
                                   Reason:
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left:10px; padding-right:10px;">
                                <%# ChangeLog.Reason %>
                            </td>
                        </tr>
                    </table>  
                    <table border="0" width="100%">
                        <tr>
                            <td class="HistoryDetailsHeading">
                                <span style="margin-left:5px">
                                   User:
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left:10px; padding-right:10px;">
                                <%# ChangeLog.UserId %>
                            </td>
                        </tr>
                    </table>  
                    <table border="0" width="100%">
                        <tr>
                            <td class="HistoryDetailsHeading">
                                <span style="margin-left:5px">
                                    Deleted Series
                                </span>
                            </td>
                        </tr>
                        <tr><td style="border:none">
                        <div style="margin-left:2px;">
                             <table width="100%" class="DuplicateDialogSeriesTable">
                                <tr class="DuplicateDialogSeriesTableHeader"><td>Description</td><td>Modality</td><td>Instances</td></tr>
                                <% foreach (SeriesInformation series in ChangeLog.Series) {%>
                                        <tr>
                                            <td><%= series.SeriesDescription %></td>
                                            <td><%= series.Modality %></td>
                                            <td><%= series.NumberOfInstances %></td>
                                        </tr>
                                <% }%>
                            </table>
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
    Collapsed="true" CollapsedText='<%# string.Format("{0} series {1} deleted. Reason: {2}", ChangeLog.Series.Count, ChangeLog.Series.Count>1? "were":"was", ChangeLog.Reason) %>'
    ExpandedText=" " TextLabelID="Label1">
</aspAjax:CollapsiblePanelExtender>
