<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemList.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.WorkQueueItemList" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Table runat="server" ID="ListContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
			Width="100%">
			<asp:TableRow VerticalAlign="top">
				<asp:TableCell ID="ListContainerCell" CssClass="GlobalGridViewPanelContent" VerticalAlign="top">
					<asp:ObjectDataSource ID="WorkQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.WorkQueueDataSource"
						DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.WorkQueueSummary" EnablePaging="true"
						SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetWorkQueueDataSource"
						OnObjectDisposing="DisposeWorkQueueDataSource"></asp:ObjectDataSource>
					<ccUI:GridView ID="WorkQueueGridView" runat="server" 
						OnRowDataBound="WorkQueueListView_RowDataBound"
						OnPageIndexChanged="WorkQueueListView_PageIndexChanged" 
						OnDataBound="WorkQueueListView_DataBound"
						OnSelectedIndexChanged="WorkQueueListView_SelectedIndexChanged"
						DataKeyNames="Key">
						<Columns>
						<asp:BoundField HeaderText="Patient ID" DataField="PatientId" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Wrap="false" ItemStyle-Wrap="false"/>
						<asp:TemplateField HeaderText="Patient Name" HeaderStyle-HorizontalAlign="Left">
								<headerstyle wrap="false" horizontalalign="Left" />
								<itemstyle wrap="false" />
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="Type" DataField="TypeString" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"/>
							<asp:TemplateField HeaderText="Schedule">
								<headerstyle wrap="false" horizontalalign="Center" />
								<itemstyle wrap="false" horizontalalign="Center" />
								<itemtemplate>
									<asp:Label ID="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledDateTime")) %>'></asp:Label>
								</itemtemplate>
							</asp:TemplateField>
						<asp:BoundField HeaderText="Priority" DataField="PriorityString" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
						<asp:BoundField HeaderText="Status" DataField="StatusString" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
							<asp:TemplateField HeaderText="Processing Server">
								<headerstyle wrap="false" horizontalalign="Center" />
								<itemstyle wrap="false" horizontalalign="Center" />
								<itemtemplate>
									<asp:Label ID="ServerInfoLabel" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
								</itemtemplate>
							</asp:TemplateField>
						<asp:BoundField HeaderText="Notes" DataField="Notes" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"/>
						</Columns>
						<EmptyDataTemplate>
                            <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No items were found using the provided criteria." />
						</EmptyDataTemplate>
						<RowStyle CssClass="GlobalGridViewRow" />
						<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
						<SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
						<HeaderStyle CssClass="GlobalGridViewHeader" />
					</ccUI:GridView>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox runat="server" ID="MessageBox" />

