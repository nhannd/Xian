<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DuplicateProcessChangeLog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DuplicateProcessChangeLog" %>


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
                                    Action: <%# ActionDescription%>
                                </span>
                            </td></tr>
                        <tr><td style="border:none">
                        <div style="margin-left:2px;">
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
    Collapsed="true" CollapsedText='<%# String.Format("{0} (Click for details)", ChangeLogShortDescription) %>'
    ExpandedText=" " TextLabelID="Label1">
</aspAjax:CollapsiblePanelExtender>
