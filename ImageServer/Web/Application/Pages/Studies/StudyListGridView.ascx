<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyListGridView"
	Codebehind="StudyListGridView.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="StudyDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.StudyDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.StudySummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetStudyDataSource"
				OnObjectDisposing="DisposeStudyDataSource"/>
				<ccUI:GridView ID="StudyListControl" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
					Width="100%" EmptyDataText=""
					CellPadding="0" OnSelectedIndexChanged="StudyListControl_SelectedIndexChanged"
					OnPageIndexChanging="StudyListControl_PageIndexChanging"
					SelectionMode="Multiple" PageSize="25" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
					BorderWidth="0px" HorizontalAlign="Left" DataSourceID="StudyDataSourceObject">
					<Columns>
						<asp:TemplateField HeaderText="Patient Name" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="Patient ID" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="AccessionNumber" HeaderText="Accession #" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center"></asp:BoundField>
						<asp:TemplateField HeaderText="Study Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                            <ccUI:DALabel ID="StudyDate" runat="server" Value='<%# Eval("StudyDate") %>'></ccUI:DALabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StudyDescription" HeaderText="Description" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="Series" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="Instances" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="ModalitiesInStudy" HeaderText="Modality" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
						<asp:BoundField DataField="StudyStatusEnumString" HeaderText="Status" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
					</Columns>
					<EmptyDataTemplate>				    
                        <ccAsp:EmptySearchResultsMessage runat="server" ID="EmptySearchResultsMessage" Message="No studies were found using the provided criteria." />
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
