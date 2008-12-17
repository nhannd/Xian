<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResultGridView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.SearchResultGridView" %>

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
					SelectionMode="Single" PageSize="25" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
					BorderWidth="0px" HorizontalAlign="Left" DataKeyNames="RowKey">
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
							ItemStyle-HorizontalAlign="Center" />
				        <asp:BoundField DataField="PartitionAE" HeaderText="Partition" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
					    
					</Columns>
					<EmptyDataTemplate>				    
					<ccAsp:EmptySearchResultsMessage runat="server" ID="NoResultFoundMessage" Message="No studies found using the provided criteria.">
						<SuggestionTemplate>					
						    <ul>
	                            <li>Modify your search criteria.</li>
	                            <li>
	                                <asp:LinkButton runat="server" PostBackUrl="~/Pages/Admin/Configure/ServerPartitions/Default.aspx" 
	                                        CssClass="EmptySearchResultsSuggestionContent">Check if audit is turned on for all partitions.</asp:LinkButton>
	                            </li>
	                        </ul>	    
						</SuggestionTemplate>
					</ccAsp:EmptySearchResultsMessage>
					
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
