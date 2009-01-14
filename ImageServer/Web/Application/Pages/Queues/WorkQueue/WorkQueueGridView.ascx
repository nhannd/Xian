<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueGridView.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.WorkQueueItemListPanel" %>
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
					<asp:GridView ID="WorkQueueListView" runat="server" CssClass="GlobalGridView" AutoGenerateColumns="false"
						PageSize="25" CellSpacing="0" CellPadding="0" AllowPaging="true" PagerSettings-Visible="false"
						CaptionAlign="Top" BorderWidth="0px" Width="100%" OnRowDataBound="WorkQueueListView_RowDataBound"
						OnPageIndexChanged="WorkQueueListView_PageIndexChanged" OnDataBound="WorkQueueListView_DataBound"
						Height="100%" OnSelectedIndexChanged="WorkQueueListView_SelectedIndexChanged"
						DataSourceID="WorkQueueDataSourceObject">
						<Columns>
						<asp:BoundField HeaderText="Patient ID" DataField="PatientId" HeaderStyle-HorizontalAlign="Left"/>
						<asp:TemplateField HeaderText="Patient Name" HeaderStyle-HorizontalAlign="Left">
								<HeaderStyle Wrap="false" HorizontalAlign="Left" />
								<ItemStyle Wrap="false" />
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="Type" DataField="TypeString" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"/>
							<asp:TemplateField HeaderText="Schedule">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledDateTime")) %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
						<asp:BoundField HeaderText="Priority" DataField="PriorityString" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
						<asp:BoundField HeaderText="Status" DataField="StatusString" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
							<asp:TemplateField HeaderText="Processing Server">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="ServerInfoLabel" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
								</ItemTemplate>
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
					</asp:GridView>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		<asp:Timer ID="RefreshTimer" runat="server" OnTick="RefreshTimer_Tick" Interval="30000">
		</asp:Timer>
	</ContentTemplate>
</asp:UpdatePanel>
<ccAsp:MessageBox runat="server" ID="MessageBox" />

