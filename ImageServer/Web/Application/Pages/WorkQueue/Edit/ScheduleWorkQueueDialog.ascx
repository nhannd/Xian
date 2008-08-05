<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ScheduleWorkQueueDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.ScheduleWorkQueueDialog" %>

<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="localAsp" %>
<%@ Register Src="../WorkQueueGridView.ascx" TagName="WorkQueueItemListPanel" TagPrefix="localAsp" %>

<ccAsp:ModalDialog id="ModalDialog" runat="server" title="Schedule Work Queue Item" Width="800px">
<ContentTemplate>
   
    <asp:Panel ID="Panel1" runat="server" CssClass="DialogPanelContent" width="100%">
        
        <localAsp:WorkQueueItemListPanel ID="WorkQueueItemListPanel" runat="server" />
        <asp:Panel runat="server" style="background-color: #eeeeee; margin: 3px;text-align: center"><center>
            <localAsp:WorkQueueSettingsPanel  ID="WorkQueueSettingsPanel" runat="server" />   
        </center>
        </asp:Panel>
       
    </asp:Panel>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel5" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="UpdateButton" OnClick="OnApplyButtonClicked" />
                            <ccUI:ToolbarButton ID="Cancel" runat="server" SkinID="CancelButton" OnClick="OnCancelButtonClicked" />
                        </asp:Panel>

                    </td>
                </tr>
            </table>
    
</ContentTemplate>
</ccAsp:ModalDialog>
<ccAsp:MessageBox id="PreOpenConfirmDialog" runat="server" />
<ccAsp:MessageBox id="PreApplyChangeConfirmDialog" runat="server" />
<ccAsp:MessageBox id="MessageDialog" runat="server" />
