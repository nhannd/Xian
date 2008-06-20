<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ScheduleWorkQueueDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.ScheduleWorkQueueDialog" %>

<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="localAsp" %>
<%@ Register Src="../WorkQueueItemListPanel.ascx" TagName="WorkQueueItemListPanel"
    TagPrefix="localAsp" %>
<ccAsp:ModalDialog id="ModalDialog" runat="server" title="Schedule Work Queue Item">
<ContentTemplate>
   
    <asp:Panel ID="Panel1" runat="server">
        
                <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelBorder">
                    <localAsp:WorkQueueItemListPanel ID="WorkQueueItemListPanel" Height="200px" runat="server" />
                </asp:Panel>
        
        <br />
        <asp:Panel ID="Panel2" runat="server" style="border-color:ThreeDShadow; border-width:1px; border-style:solid;">
                    <localAsp:WorkQueueSettingsPanel  ID="WorkQueueSettingsPanel" runat="server" />    
                </asp:Panel>
        <br />
        
        <asp:Panel ID="Panel3" runat="server" style="padding:5px; text-align:center">
            <table cellspacing="10px">
                <tr>
                    <td>
                        <asp:Button ID="ApplyButton" runat="server" Text="Apply" Width="100px" OnClick="OnApplyButtonClicked"/>
                    </td>
                    <td>
                        <asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="100px"  OnClick="OnCancelButtonClicked" />
                    </td>
                </tr>
            </table>
            
            
        </asp:Panel>
    </asp:Panel>
    
</ContentTemplate>
</ccAsp:ModalDialog>
<ccAsp:MessageBox id="PreOpenConfirmDialog" runat="server" />
<ccAsp:MessageBox id="PreApplyChangeConfirmDialog" runat="server" />
<ccAsp:MessageBox id="MessageDialog" runat="server" />
