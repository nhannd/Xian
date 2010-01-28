<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertIndicator.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.AlertIndicator" %>

<div id="AlertLinkPanel" >| <asp:LinkButton ID="AlertLink" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" CssClass="UserInformationLink">Critical Alerts: <asp:Label ID="AlertsCount" runat="server" /></asp:LinkButton>
<div id="AlertDetailsPanel" class="AlertDetailsPanel" style="display: none">
    <div>
        <asp:Table runat="server" ID="AlertTable" style="background: white; border: lightsteelblue 1px solid; padding: 2px;">
            <asp:TableRow CssClass="AlertTableHeaderCell"><asp:TableCell>Component</asp:TableCell><asp:TableCell>Source</asp:TableCell><asp:TableCell>Description</asp:TableCell></asp:TableRow>
        </asp:Table>
    </div>
    <div style="text-align: right; padding: 0px 2px 0px 0px; margin-top: 2px; font-weight: bold;"><table><tr><td nowrap="nowrap"><asp:LinkButton ID="LinkButton2" runat="server" PostBackUrl="~/Pages/Admin/Alerts/Default.aspx" style="color: #6699CC; text-decoration: none;">View All Alerts</asp:LinkButton></td><td> | </td><td><a id="CloseButton" href="" style="color: #6699CC; text-decoration: none;">Close</a></td></tr></table></div>
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