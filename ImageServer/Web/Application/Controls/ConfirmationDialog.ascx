<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ConfirmationDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ConfirmationDialog" %>

<%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="localAsp" %>

<localAsp:ModalDialog ID="ModalDialog" runat="server" Title="" >
    <ContentTemplate>
        <asp:ScriptManagerProxy ID="DialogScriptManager" runat="server"/>
                <table cellspacing="0" cellpadding="0">
                    <tr>
                        <td colspan="1" style="height: 24px">
                            <asp:Image ID="IconImage" runat="server" Visible="false" /></td>
                        <td colspan="2" style="height: 24px; vertical-align: top; text-align: center;">
                            <asp:Panel runat="server" CssClass="ConfirmationContent">
                                <asp:Label ID="MessageLabel" runat="server" Style="text-align: center" Text="Message" />
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <asp:Panel ID="ButtonPanel" runat="server" DefaultButton="NoButton" CssClass="ConfirmationButtonPanel">
                                            <ccUI:ToolbarButton ID="YesButton" runat="server" SkinID="YesButton" OnClick="YesButton_Click" />
                                            <ccUI:ToolbarButton ID="NoButton" runat="server" SkinID="NoButton" OnClick="NoButton_Click"  />
                                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OKButton" OnClick="OKButton_Click"  />
                                            <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                           
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</localAsp:ModalDialog>
