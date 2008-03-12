<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ConfirmationDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog" %>
<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="uc1" %>
<uc1:ModalDialog ID="ModalDialog1" runat="server" Title="Please Confirm" BackgroundCSS="CSSModalBackground">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server">
            <asp:Panel ID="Panel1" runat="server">
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" /></td>
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
                            <table width="50%">
                                <tr>
                                    <td>
                                        <asp:Button ID="YesButton" runat="server" OnClick="YesButton_Click" Text="Yes" Width="77px" />
                                    </td>
                                    <td>
                                        <asp:Button ID="NoButton" runat="server" Text="No" OnClick="NoButton_Click" Width="77px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</uc1:ModalDialog>
