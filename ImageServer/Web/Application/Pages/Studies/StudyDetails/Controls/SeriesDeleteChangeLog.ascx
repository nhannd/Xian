<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDeleteChangeLog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.SeriesDeleteChangeLog" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Edit" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Core.Data" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common.CommandProcessor" %>

<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=HistoryDetailsPanel.ClientID%>").hide();
        $("#<%=ShowHideDetails.ClientID%>").click(function() {
            if ($("#<%=ShowHideDetails.ClientID%>").text() == "[Show Details]") {
                $("#<%=HistoryDetailsPanel.ClientID%>").show();
                $("#<%=ShowHideDetails.ClientID%>").text("[Hide Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "bold");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "5px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            } else {
                $("#<%=HistoryDetailsPanel.ClientID%>").hide();
                $("#<%=ShowHideDetails.ClientID%>").text("[Show Details]");
                $("#<%=SummaryPanel.ClientID %>").css("font-weight", "normal");
                $("#<%=SummaryPanel.ClientID %>").css("margin-top", "0px");
                $("#<%=ShowHideDetails.ClientID%>").css("font-weight", "normal");
            }
            return false;
        });
    });

</script>

<div runat="server" id="SummaryPanel">
    <%# string.Format("{0} series {1} deleted by {2}", ChangeLog.Series.Count, ChangeLog.Series.Count>1? "were":"was", ChangeLog.UserId ?? "Unknown") %>
    <a href="#" id="ShowHideDetails" style="margin-left: 5px;" runat="server">[Show Details]</a>
</div>
<div id="HistoryDetailsPanel" runat="server" class="HistoryDetailsPanel">
    <table class="ReasonSummary" cellspacing="0" cellpadding="0">
        <tr>
            <td class="HistoryDetailsLabel">
                Reason:
            </td>
            <td align="left">
                <%= GetReason(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel">
                Comment:
            </td>
            <td align="left">
                <%= GetComment(ChangeLog.Reason) %>
            </td>
        </tr>
        <tr>
            <td class="HistoryDetailsLabel" colspan="2" nowrap="nowrap" style="padding-top: 8px;">
                Deleted&nbsp;Series
            </td>
        </tr>
    </table>
    <div style="border-bottom: dashed 1px #999999; margin-top: 3px;">
    </div>
    <div style="padding: 2px;">
    <table width="100%" cellspacing="0">
        <tr style="color: #205F87; background: #eeeeee; padding-top: 2px;">
            <td>
                <b>Description</b>
            </td>
            <td>
                <b>Modality</b>
            </td>
            <td>
                <b>Instances</b>
            </td>
        </tr>
        <% foreach (SeriesInformation series in ChangeLog.Series)
           {%>
        <tr style="background: #fefefe">
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.SeriesDescription %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.Modality %>
            </td>
            <td style="border-bottom: solid 1px #dddddd">
                <%= series.NumberOfInstances %>
            </td>
        </tr>
        <% }%>
    </table>
    </div>
</div>