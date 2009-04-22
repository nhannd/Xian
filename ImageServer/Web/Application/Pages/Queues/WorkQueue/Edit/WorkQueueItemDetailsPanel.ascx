<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel" %>

<script type="text/javascript">
    Sys.Application.add_load(function(){
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").hide();
        });
        prm.add_endRequest(function(){
            $("#<%= AutoRefreshIndicator.ClientID %>").show();
        });
    });
    
</script>
<asp:Panel ID="Panel1" runat="server">
     <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
         
           <table width="100%" cellpadding="0" cellspacing="0">
                <tr  class="MainContentTitle">
                    <td>
                        <asp:Label ID="WorkQueueItemTitle" runat="server" Text="Work Queue Item Details"></asp:Label>
                    </td>
                    <td align="right">                        
                         <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel" DisplayAfter="0">
                            <ProgressTemplate>
                                <asp:Image ID="RefreshingIndicator" runat="server" SkinID="AjaxLoadingBlue" />
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:Image ID="AutoRefreshIndicator" runat="server" SkinID="RefreshEnabled" />
                            
                    </td>
                </tr>
                <tr><td colspan="2">
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td>
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" SkinID="RescheduleButton" OnClick="Reschedule_Click"/>
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" SkinID="ResetButton" OnClick="Reset_Click"/>
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="DeleteButton" OnClick="Delete_Click"/>
                                    <ccUI:ToolbarButton ID="ReprocessButton" runat="server" SkinID="ReprocessButton" OnClick="Reprocess_Click"/>
                                    
                            </td></tr>
                            <tr><td><asp:PlaceHolder ID="WorkQueueDetailsViewPlaceHolder" runat="server"></asp:PlaceHolder></td></tr>
                       </table>
                  </td></tr>
              </table>
              
              
            <ccUI:Timer ID="RefreshTimer" runat="server" Interval="10000" OnTick="RefreshTimer_Tick" OnAutoDisabled="OnAutoRefreshDisabled" DisableAfter="3"></ccUI:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
