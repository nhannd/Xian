<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScheduleWorkQueueDialog.ascx.cs" 
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ScheduleWorkQueueDialog" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog"
    TagPrefix="uc2" %>
<%@ Register Src="~/Common/ModalDialog.ascx" TagName="ModalDialog" TagPrefix="uc1" %>
<%@ Register Src="WorkQueueSettingsPanel.ascx" TagName="WorkQueueSettingsPanel" TagPrefix="uc1" %>
<%@ Register Src="~/WorkQueue/WorkQueueItemListPanel.ascx" TagName="WorkQueueItemListPanel" TagPrefix="clearcanvas" %>

<uc1:ModalDialog ID="ModalDialog1" runat="server" Title="Schedule Work Queue Item" BackgroundCSS="CSSDefaultPopupDialogBacground" >
<ContentTemplate>
    <asp:Panel ID="Panel1" runat="server">
    
        <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelBorder">
            <clearcanvas:WorkQueueItemListPanel ID="WorkQueueItemListPanel" Height="200px" runat="server"></clearcanvas:WorkQueueItemListPanel>
        </asp:Panel>
        <br />
        <asp:Panel ID="Panel2" runat="server" style="border-color:ThreeDShadow; border-width:1px; border-style:solid;">
            <uc1:WorkQueueSettingsPanel ID="WorkQueueSettingsPanel" runat="server" />    
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
</uc1:ModalDialog>

<uc2:ConfirmationDialog ID="PreOpenConfirmDialog" runat="server" />
<uc2:ConfirmationDialog ID="PreApplyChangeConfirmDialog" runat="server" />
<uc2:ConfirmationDialog ID="InformationDialog" runat="server" />
