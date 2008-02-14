<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ConfirmDialog" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="DialogPanel" runat="server" CssClass="CSSPopupWindow" Style="display: none;">
            <asp:Table runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell>
                        <asp:Table runat="server" CssClass="CSSPopupWindowTitleBar" CellPadding="0" CellSpacing="0" BorderWidth="1">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label ID="TitleLabel" runat="server" Text="Please Confirm!"></asp:Label>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <div class="CSSPopupWindowBody">
                                <table id="TABLE1" runat="server" cellspacing="0" cellpadding="0"
                                    >
                                    <tr>
                                        <td colspan="1" style="height: 24px">
                                            <asp:Image ID="IconImage" runat="server" /></td>
                                        <td colspan="2" style="height: 24px; padding-right: 10px; padding-left: 10px; padding-bottom: 10px;
                                            vertical-align: top; padding-top: 10px; text-align: center;">
                                            <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message"
                                                Width="100%"></asp:Label></td>
                                    </tr>
                                </table>
                                
                            <asp:Panel ID="DummyPanel" runat="server" Height="1px" Style="z-index: 100; left: 18px;
                                position: absolute; top: 250px">
                            </asp:Panel>
                        </div>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="center">
                        <table cellpadding="5" width="30%">
                            <tr>
                                <td>
                                    <asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Text="Yes" Width="77px" />
                                </td>
                                <td>
                                    <asp:Button ID="CancelButton" runat="server" Text="Cancel"/>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br />
        </asp:Panel>
        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="DummyPanel"
            PopupControlID="DialogPanel" Drag="true" DropShadow="true" PopupDragHandleControlID="TitleLabel"
            BackgroundCssClass="CSSModalBackground">
        </ajaxToolkit:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
