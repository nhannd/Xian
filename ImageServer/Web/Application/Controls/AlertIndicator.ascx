<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertIndicator.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.AlertIndicator" %>

<style>
.AlertTableHeaderCell td
{
    font-weight: bold;
    border-bottom: solid 1px black;
    padding-left: 5px; 
    padding-right: 5px;
    padding-bottom: 2px; 
}

.AlertTableCell td
{
    border-bottom: solid 1px #cccccc;
    padding: 2px 5px 3px 5px;
    color: #333333;
}

.AlertDetailsPanel
{
    top: 19px; 
    right: 0px; 
    position: absolute; 
    z-index: 2; 
    border: solid 2px #6699CC;
    background: white; 
    text-align: left;
}
</style>

<div id="AlertLinkPanel" >| <asp:LinkButton ID="AlertLink" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" CssClass="UserInformationLink">Alerts: <asp:Label ID="AlertsCount" runat="server" /></asp:LinkButton>
<div id="AlertDetailsPanel" class="AlertDetailsPanel" style="display: none">
    <div style="padding: 3px 5px 3px 5px;">
        <asp:Table runat="server" ID="AlertTable">
            <asp:TableRow CssClass="AlertTableHeaderCell"><asp:TableCell>Component</asp:TableCell><asp:TableCell>Source</asp:TableCell><asp:TableCell>Description</asp:TableCell></asp:TableRow>
        </asp:Table>
    </div>
    <div style="text-align: right; padding: 3px 7px 5px 0px; margin-top: 3px; background: #eeeeee; font-weight: bold;"><table><tr><td nowrap="nowrap"><asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" style="color: #6699CC; text-decoration: none;">View All Alerts</asp:LinkButton></td><td> | </td><td><a id="CloseButton" href="" style="color: #6699CC; text-decoration: none;">Close</a></td></tr></table></div>
</div>
</div>

<% if(alerts.Count > 0) { %>        
<script type="text/javascript">

    $(document).ready(function() {
        $("#<%=AlertLink.ClientID %>").mouseover(function() { 
            $(".AlertDetailsPanel:hidden").show();
        });
        $("#CloseButton").click(function(event) { 
            event.preventDefault();
            $("#AlertDetailsPanel:visible").slideUp("fast");
        });             
    });
</script>
<%} %>