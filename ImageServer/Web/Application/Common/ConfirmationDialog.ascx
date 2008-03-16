<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ConfirmationDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="uc1" %>
<uc1:ModalDialog ID="ModalDialog" runat="server" Title="Please Confirm" BackgroundCSS="CSSDefaultPopupDialogBacground">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server">
            <asp:Panel ID="Panel1" runat="server">
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" Visible="false" /></td>
                        <td colspan="2" style="height: 24px; padding-right: 10px; padding-left: 10px; padding-bottom: 10px;
                            vertical-align: top; padding-top: 10px; text-align: center;">
                            <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message">
                            </asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="center">
                            <asp:Panel ID="ButtonPanel" runat="server">
                                 <table width="50%" cellspacing="10">
                                    <tr>
                                        <td>
                                            <asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Text="Yes" Width="77px" />
                                        </td>
                                        <td>
                                            <asp:Button ID="NoButton" runat="server" Text="No" OnClick="NoButton_Click" Width="77px" />
                                        </td>
                                        <td>
                                            <asp:Button ID="OKButton" runat="server" Text="OK" OnClick="OKButton_Click" Width="77px" />
                                        </td>
                                        <td>
                                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" Width="77px" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                           
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</uc1:ModalDialog>
