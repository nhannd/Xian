<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.Move.StudyGridView" %>

            <asp:GridView ID="StudyListControl" runat="server" AutoGenerateColumns="False"
                CssClass="GlobalGridView" Width="100%" EmptyDataText="" CellPadding="0" 
                PageSize="10" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
                BorderWidth="0px">
                <Columns>
                    <asp:TemplateField HeaderText="Patient Name">
                        <itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PatientId" HeaderText="Patient ID"></asp:BoundField>
                    <asp:BoundField DataField="AccessionNumber" HeaderText="Accession #" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:TemplateField HeaderText="Study Date">
                        <itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>' HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></ccUI:DALabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StudyDescription" HeaderText="Description"></asp:BoundField>
                                    </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                        CssClass="GlobalGridViewHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>Patient Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Patient ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Accession #</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Study Date</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Description</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

