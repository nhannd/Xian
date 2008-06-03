<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.StudyListGridView"
    Codebehind="StudyListGridView.ascx.cs" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <ccUI:GridView ID="StudyListControl" runat="server" AutoGenerateColumns="False"
                CssClass="GlobalGridView" Width="100%" OnRowDataBound="StudyListControl_RowDataBound"
                OnDataBound="StudyListControl_DataBound" EmptyDataText="" CellPadding="0" OnSelectedIndexChanged="StudyListControl_SelectedIndexChanged"
                OnSelectedIndexChanging="StudyListControl_SelectedIndexChanging" OnPageIndexChanging="StudyListControl_PageIndexChanging"
                SelectionMode="Multiple" PageSize="20" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
                BorderWidth="0px" HorizontalAlign="Left">
                <Columns>
                    <asp:TemplateField HeaderText="Patient Name" HeaderStyle-HorizontalAlign="Left" >
                        <itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PatientId" HeaderText="Patient ID" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:BoundField DataField="AccessionNumber" HeaderText="Accession #" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Study Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                        </itemtemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StudyDescription" HeaderText="Description" HeaderStyle-HorizontalAlign="Left">
                    </asp:BoundField>
                    <asp:BoundField DataField="NumberOfRelatedSeries" HeaderText="Series" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField DataField="NumberOfRelatedInstances" HeaderText="Instances" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField DataField="ModalitiesInStudy" HeaderText="Modality" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="Status" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                        CssClass="GlobalGridViewHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell HorizontalAlign="Left">Patient Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Left">Patient ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Accession #</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Study Date</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Left">Description</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Series</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Instances</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Modality</asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">Status</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
