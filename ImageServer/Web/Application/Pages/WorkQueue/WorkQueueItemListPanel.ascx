<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemListPanel.ascx.cs"
	Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.WorkQueueItemListPanel" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
		<asp:Table runat="server" ID="ListContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
			Width="100%">
			<asp:TableRow VerticalAlign="top">
				<asp:TableCell ID="ListContainerCell" CssClass="GlobalGridViewPanelContent" VerticalAlign="top">
					<asp:ObjectDataSource ID="WorkQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.WorkQueueDataSource"
						DataObjectTypeName="ClearCanvas.ImageServer.Model.WorkQueue" EnablePaging="true"
						SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetWorkQueueDataSource"
						OnObjectDisposing="DisposeWorkQueueDataSource"></asp:ObjectDataSource>
					<asp:GridView ID="WorkQueueListView" runat="server" CssClass="GlobalGridView" AutoGenerateColumns="false"
						PageSize="25" CellSpacing="0" CellPadding="0" AllowPaging="true" PagerSettings-Visible="false"
						CaptionAlign="Top" BorderWidth="0px" Width="100%" OnRowDataBound="WorkQueueListView_RowDataBound"
						OnPageIndexChanged="WorkQueueListView_PageIndexChanged" OnDataBound="WorkQueueListView_DataBound"
						Height="100%" OnSelectedIndexChanged="WorkQueueListView_SelectedIndexChanged"
						DataSourceID="WorkQueueDataSourceObject">
						<Columns>
							<asp:TemplateField HeaderText="Patient ID">
								<HeaderStyle Wrap="false" HorizontalAlign="Left" />
								<ItemStyle Wrap="false" />
								<ItemTemplate>
									<asp:Label ID="PatientId" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Patient Name">
								<HeaderStyle Wrap="false" HorizontalAlign="Left" />
								<ItemStyle Wrap="false" />
								<ItemTemplate>
									<ccUI:PersonNameLabel ID="PatientName" runat="server" PersonNameType="Dicom"></ccUI:PersonNameLabel>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Type">
								<HeaderStyle Wrap="false" HorizontalAlign="Left" />
								<ItemStyle Wrap="false" HorizontalAlign="Left" />
								<ItemTemplate>
									<asp:Label ID="Type" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Schedule">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledTime")) %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Priority">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="Priority" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Status">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="Status" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Processing Server">
								<HeaderStyle Wrap="false" HorizontalAlign="Center" />
								<ItemStyle Wrap="false" HorizontalAlign="Center" />
								<ItemTemplate>
									<asp:Label ID="ServerInfoLabel" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
							<asp:TemplateField HeaderText="Notes">
								<HeaderStyle Wrap="false" HorizontalAlign="Left" />
								<ItemStyle Wrap="true" HorizontalAlign="Left" />
								<ItemTemplate>
									<asp:Label ID="Notes" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateField>
						</Columns>
						<EmptyDataTemplate>
							<asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
								CssClass="GlobalGridViewHeader">
								<asp:TableHeaderRow>
									<asp:TableHeaderCell>Patient ID</asp:TableHeaderCell>
									<asp:TableHeaderCell>Patient Name</asp:TableHeaderCell>
									<asp:TableHeaderCell>Type</asp:TableHeaderCell>
									<asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
									<asp:TableHeaderCell>Priority</asp:TableHeaderCell>
									<asp:TableHeaderCell>Status</asp:TableHeaderCell>
									<asp:TableHeaderCell>Processing Server</asp:TableHeaderCell>
									<asp:TableHeaderCell>Notes</asp:TableHeaderCell>
								</asp:TableHeaderRow>
							</asp:Table>
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
