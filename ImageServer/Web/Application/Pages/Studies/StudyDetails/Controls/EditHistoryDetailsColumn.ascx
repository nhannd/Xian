<%@ Import namespace="ClearCanvas.ImageServer.Core.Edit"%>
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
                              {%>
                                  <table width="100%">
                                    <tr><td><b>Tag</b></td><td><b>Original Value</b></td><td><b>New Value</b></td></tr>
                                  <%{
                                        foreach(BaseImageLevelUpdateCommand cmd in EditHistory.UpdateCommands)
                                        {
                                              IUpdateImageTagCommand theCmd = cmd as IUpdateImageTagCommand;
                                              if (theCmd != null)
                                              { %><tr>
                                                <td><pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.TagPath.Tag) %></pre></td>
                                                <td><pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.OriginalValue) %></pre></td>
                                                <td><pre><%= HtmlUtility.Encode(theCmd.UpdateEntry.Value!=null? theCmd.UpdateEntry.Value.ToString(): "") %></pre></td>
                                                </tr>
                                              <%} %>
                                      <%}%>
                                  <%}%>
                                  </table>                                 
                                  
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
