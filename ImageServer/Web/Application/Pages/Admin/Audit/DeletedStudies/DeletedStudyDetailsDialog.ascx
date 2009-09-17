<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.Model"%>
<%@ Import namespace="System.ComponentModel"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Application.Helpers"%>
<%@ Import namespace="Microsoft.JScript"%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeletedStudyDetailsDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialog" %>
<%@ Register Src="DeletedStudyDetailsDialogPanel.ascx" TagName="DialogContent" TagPrefix="localAsp" %>

<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="775px">
    <ContentTemplate>
        
        <localAsp:DialogContent runat="server" ID="DialogContent" />
        
        <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="CloseButton" runat="server" SkinID="OKButton" ValidationGroup="DeleteStudyDialogValidationGroup" OnClick="CloseClicked" /> 
                        </asp:Panel>

                    </td>
                </tr>
            </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
