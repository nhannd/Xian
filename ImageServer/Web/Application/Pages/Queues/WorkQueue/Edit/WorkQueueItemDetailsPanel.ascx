<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel" %>

<asp:Panel ID="Panel1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td class="MainContentTitle"><asp:Label ID="WorkQueueItemTitle" runat="server" Text="Work Queue Item Details"></asp:Label></td></tr>
                <tr><td>
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
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Timer ID="RefreshTimer" runat="server" Interval="10000" OnTick="RefreshTimer_Tick">
    </asp:Timer>
</asp:Panel>
