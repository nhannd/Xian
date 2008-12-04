<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeletedStudyDetailsDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudyDetailsDialog" %>
<ccAsp:ModalDialog ID="ModalDialog" runat="server" Width="775px">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server">
            <table cellpadding="2" cellspacing="5" width="100%" style="background-color: #eeeeee;
                border: solid 1px #cccccc;">
                <tr>
                <td>
                <asp:DetailsView ID="StudyDetailView" runat="server" AutoGenerateRows="False" GridLines="Horizontal"
                    CellPadding="4" CssClass="GlobalGridView"
                    Width="100%" >
                    <Fields>
                        <asp:TemplateField HeaderText="Patient's Name: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:PersonNameLabel ID="PatientsName" runat="server" 
                                    PersonName='<%# Eval("PatientsName") %>'
                                    PersonNameType="Dicom"></ccUI:PersonNameLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PatientId" HeaderText="Patient Id: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="StudyDescription" HeaderText="Study Description: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="AccessionNumber" HeaderText="Accession Number: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Study Date/Time: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="StudyInstanceUid" HeaderText="Study Instance UID: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PartitionAE" HeaderText="Partition AE: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DeletedFolderPath" HeaderText="Backup Folder:">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ReasonForDeletion" HeaderText="Reason For Deletion:">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Delete Date/Time: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                            <ccUI:DateTimeLabel ID="DeleteDate" runat="server"  Value='<%# Eval("DeleteTime") %>'></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <RowStyle CssClass="GlobalGridViewRow" />
                    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                </asp:DetailsView>
                </td>
                </tr>
                
            </table>
        </asp:Panel>
        
        <table cellpadding="0" cellspacing="0" width="100%">
                <tr align="right">
                    <td>
                        <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                            <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="OkButton" ValidationGroup="vg1" OnClick="OKClicked" /> 
                        </asp:Panel>

                    </td>
                </tr>
            </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
