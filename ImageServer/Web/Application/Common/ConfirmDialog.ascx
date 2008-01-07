<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.ConfirmDialog" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Style="display: none;" Width="350px">
    <asp:Panel ID="TitleBarPanel" runat="server" CssClass="CSSPopupWindowTitleBar">
        <table style="width: 100%">
            <tr>
                <td valign="middle">
    <asp:Label ID="TitleLabel" runat="server"
        Text="Please Confirm!" Width="100%"></asp:Label></td>
                
            </tr>
        </table>
    </asp:Panel>
    <div class="CSSPopupWindowBody">
        <table id="TABLE1" align="center" runat="server" cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <td colspan="1" style="height: 24px">
                    <asp:Image ID="IconImage" runat="server" /></td>
                <td colspan="2" style="height: 24px; padding-right: 10px; padding-left: 10px; padding-bottom: 10px; vertical-align: top; padding-top: 10px; text-align: center;">
                    <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message"
                        Width="100%"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="1" style="height: 24px">
                </td>
                <td style="height: 24px" colspan="2"></td>
            </tr>
        </table>
        
        <table cellpadding="5" width="100%">
            <tr>
                <td style="text-align: center">
                    &nbsp;<asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Text="Yes" Width="77px" />
                    &nbsp; &nbsp; &nbsp;
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
            </tr>
        </table>
        
        <asp:Panel ID="DummyPanel" runat="server" Height="1px" Style="z-index: 100; left: 18px;
            position: absolute; top: 250px" >
        </asp:Panel>
    </div>
</asp:Panel>
        
<cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="DummyPanel" PopupControlID="DialogPanel" 
        Drag="true" DropShadow="true"  PopupDragHandleControlID="TitleLabel" BackgroundCssClass ="CSSModalBackground"
        >
</cc1:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
