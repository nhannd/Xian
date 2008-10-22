<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.RestoreQueue.RestoreQueueItemList"
	Codebehind="RestoreQueueItemList.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="RestoreQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.RestoreQueueDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.RestoreQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetRestoreQueueDataSource"
				OnObjectDisposing="DisposeRestoreQueueDataSource"/>
				<ccUI:GridView ID="RestoreQueueGridView" runat="server" SkinID="CustomGlobalGridView"
					OnSelectedIndexChanged="RestoreQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="RestoreQueueGridView_PageIndexChanging"
					SelectionMode="Multiple" DataSourceID="RestoreQueueDataSourceObject">
					<Columns>
						<asp:TemplateField HeaderText="Patient Name" HeaderStyle-HorizontalAlign="Left">
							<itemtemplate>
                            <ccUI:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientsName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
                        </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="PatientId" HeaderText="Patient ID" HeaderStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:TemplateField HeaderText="Scheduled Time" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <ccUI:DateTimeLabel ID="ScheduledDateTime" Value='<%# Eval("ScheduledDateTime") %>' runat="server"></ccUI:DateTimeLabel>
                            </itemtemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="StatusString" HeaderText="Status" HeaderStyle-HorizontalAlign="Center"
							ItemStyle-HorizontalAlign="Center" />
					</Columns>
					<EmptyDataTemplate>
						<asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
							CssClass="GlobalGridViewHeader">
							<asp:TableHeaderRow>
								<asp:TableHeaderCell HorizontalAlign="Left">Patient Name</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Left">Patient ID</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Center">Scheduled Time</asp:TableHeaderCell>
								<asp:TableHeaderCell HorizontalAlign="Left">Status</asp:TableHeaderCell>
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
