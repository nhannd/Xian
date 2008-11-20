<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/Common/MainContentSection.Master" Title="ClearCanvas ImageServer" Codebehind="WorkQueueItemDeleted.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDeleted" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server"><asp:TableRow><asp:TableCell HorizontalAlign="right" style="padding-top: 12px;"><asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" OnClientClick="javascript: window.close(); return false;" /></asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentSectionPlaceHolder" >
    <table width="100%" cellpadding="0" cellspacing="0">
                <tr><td class="MainContentTitle"><asp:Label ID="WorkQueueItemTitle" runat="server" Text="Work Queue Item Details"></asp:Label></td></tr>
                <tr><td>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td>
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" SkinID="RescheduleButton"/>
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" SkinID="ResetButton" />
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" SkinID="DeleteButton" />
                                    <ccUI:ToolbarButton ID="ReprocessButton" runat="server" SkinID="ReprocessButton"/>                                    
                            </td></tr>
                            <tr><td>
                                <asp:Panel ID="Panel1" runat="server" BackColor="white" style="padding: 10px; border: solid 1px #305ba6">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="WorkQueueItemDeletedMessage" style="margin: 0px;">
                                        The item has been successfully deleted from the Work Queue.
                                    </asp:Panel>
                                </asp:Panel>                            
                            </td></tr>
                       </table>
                  </td></tr>                    
              </table>
 </asp:Content>