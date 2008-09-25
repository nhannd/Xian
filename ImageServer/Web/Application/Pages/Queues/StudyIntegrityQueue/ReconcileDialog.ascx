<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.ReconcileDialog"
    Codebehind="ReconcileDialog.ascx.cs" %>

<ccAsp:ModalDialog ID="ReconcileItemModalDialog" runat="server" Width="800px">
    <ContentTemplate> 
        <div style="background:#CCCCCC; border: solid 1px #205F87">
            <asp:Table runat="server">
                <asp:TableRow style="background: #CCCCCC; padding: 4px; text-align: center; font-weight:bold;">
                    <asp:TableCell >Existing Study</asp:TableCell><asp:TableCell style="background: #CCCCCC; border-left: solid 2px #205F87;">Conflicting Study</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2" style="text-align: center; padding: 4px; background-color: #eeeeee; border-top: solid 1px black; border-bottom: solid 1px black;">Study Instance UID: <asp:Label ID="StudyInstanceUIDLabel" runat="server"></asp:Label></asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell style="vertical-align: top;">
                        <asp:Table runat="server" style="" Height="100%">
                            <asp:TableRow style="text-align: center; padding: 4px; background: #CCCCCC;">
                                <asp:TableCell style="border-bottom: solid 1px black;"><asp:Label ID="ExistingNameLabel" runat="server"></asp:Label></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div style="width: 100%; height: 200px; overflow: scroll; background: #FFFFFF;"><asp:GridView runat="server" ID="ExistingPatientSeriesGridView" width="378px" BackColor="white" GridLines="Horizontal" RowHeaderColumn="false" BorderColor="Transparent" RowStyle-BorderColor="#CCCCCC" RowStyle-BorderStyle="solid" RowStyle-BorderWidth="2px" RowStyle-HorizontalAlign="Center" HeaderStyle-BackColor="#B8D8EE" HeaderStyle-ForeColor="#205F87" /></div>
                                </asp:TableCell>
                             </asp:TableRow>
                             <asp:TableRow style="background: #EEEEEE" Width="100%"><asp:TableCell></asp:TableCell></asp:TableRow>
                        </asp:Table>
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Table runat="server">
                            <asp:TableRow style="text-align: center; padding: 4px; vertical-align: top; background: #CCCCCC">
                                <asp:TableCell style="border-bottom: solid 1px black; border-left: solid 2px #205F87;"><asp:Label ID="ConflictingNameLabel" runat="server"></asp:Label></asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>
                                    <div style="width: 100%; height: 200px; overflow: scroll; background: #FFFFFF; border-left: solid 2px #205F87;">
                                        <asp:GridView runat="server" ID="ConflictingPatientSeriesGridView" width="378px" BackColor="white" GridLines="Horizontal" RowHeaderColumn="false" BorderColor="Transparent" RowStyle-BorderColor="#CCCCCC" RowStyle-BorderStyle="solid" RowStyle-BorderWidth="2px" RowStyle-HorizontalAlign="Center" HeaderStyle-BackColor="#B8D8EE" HeaderStyle-ForeColor="#205F87">
                                        
                                        </asp:GridView>
                                    </div>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow style="text-align: center; padding: 4px; background: #CCCCCC;"> 
                                <asp:TableCell style="border-left: solid 2px #205F87;"><asp:LinkButton ID="MergeButton" runat="server" OnClick="MergeStudyButton_Click" style="color: Red;" /><asp:LinkButton runat="server" OnClick="NewStudyButton_Click" style="color: Red;">Create New Study</asp:LinkButton> | <asp:LinkButton runat="server" OnClick="DiscardButton_Click" style="color: Red">Discard</asp:LinkButton></asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>                                    
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td align="right">
                            <asp:Panel runat="server" CssClass="DefaultModalDialogButtonPanel">
                                <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="CancelButton" OnClick="CancelButton_Click" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
