<%@ Import namespace="ClearCanvas.ImageServer.Web.Application.Helpers"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.Model"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyDetailsDialogGeneralPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyDetailsDialogGeneralPanel" %>
<asp:Panel ID="Panel3" runat="server">
    <table width="100%">
        <tr>
            <td>
                <asp:DetailsView ID="StudyDetailView" runat="server" AutoGenerateRows="False" GridLines="Horizontal"
                    CellPadding="4" CssClass="GlobalGridView" Width="100%">
                    <Fields>
                        <asp:TemplateField HeaderText="Patient's Name: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:PersonNameLabel ID="PatientsName" runat="server" PersonName='<%# Eval("PatientsName") %>'
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
                        <asp:BoundField DataField="BackupFolderPath" HeaderText="Backup Location:">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Reason For Deletion: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <%# HtmlEncoder.EncodeText((Container.DataItem as DeletedStudyInfo).ReasonForDeletion)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delete Date/Time: ">
                            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="DeleteDate" runat="server" Value='<%# Eval("DeleteTime") %>'></ccUI:DateTimeLabel>
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
