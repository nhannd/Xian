<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResultGridView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.SearchResultGridView" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
		
            <asp:ObjectDataSource ID="DataSource" runat="server" 
                TypeName="ClearCanvas.ImageServer.Web.Common.Data.DeletedStudyDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.Model.DeletedStudyInfo" 
				EnablePaging="true"
				SelectMethod="Select"
				StartRowIndexParameterName="startRowIndex"
				MaximumRowsParameterName="maxRows"
				SelectCountMethod="SelectCount" />
				
				<ccUI:GridView ID="ListControl" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
					Width="100%" EmptyDataText=""
					CellPadding="0"  DataSourceID="DataSource"
					SelectionMode="Multiple" PageSize="25" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
					BorderWidth="0px" HorizontalAlign="Left">
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
						<asp:BoundField DataField="StudyDescription" HeaderText="Description" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" /><%--
						<asp:BoundField DataField="StudyInstanceUid" HeaderText="Study Instance Uid" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />--%>
				        <asp:BoundField DataField="PartitionAE" HeaderText="Partition" HeaderStyle-HorizontalAlign="Center"
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
