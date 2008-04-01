<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ScheduleWorkQueueDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ScheduleWorkQueueDialog" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog"
    TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/ModalDialog.ascx" TagName="ModalDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="clearcanvas" %>
<%@ Register Src="~/WorkQueue/WorkQueueItemListPanel.ascx" TagName="WorkQueueItemListPanel"
    TagPrefix="clearcanvas" %>
<clearcanvas:modaldialog id="ModalDialog" runat="server" title="Schedule Work Queue Item"
    backgroundcss="CSSDefaultPopupDialogBackground">
<ContentTemplate>
   
    <asp:Panel ID="Panel1" runat="server">
        
                <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelBorder">
                    <clearcanvas:WorkQueueItemListPanel ID="WorkQueueItemListPanel" Height="200px" runat="server" />
                </asp:Panel>
        
        <br />
        <asp:Panel ID="Panel2" runat="server" style="border-color:ThreeDShadow; border-width:1px; border-style:solid;">
                    <clearcanvas:WorkQueueSettingsPanel  ID="WorkQueueSettingsPanel" runat="server" />    
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
</clearcanvas:modaldialog>
<clearcanvas:confirmationdialog id="PreOpenConfirmDialog" runat="server" />
<clearcanvas:confirmationdialog id="PreApplyChangeConfirmDialog" runat="server" />
<clearcanvas:confirmationdialog id="InformationDialog" runat="server" />
