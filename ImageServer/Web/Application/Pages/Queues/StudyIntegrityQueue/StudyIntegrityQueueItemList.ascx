<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.StudyIntegrityQueueItemList"
	Codebehind="StudyIntegrityQueueItemList.ascx.cs" %>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">
			<asp:ObjectDataSource ID="StudyIntegrityQueueDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.StudyIntegrityQueueSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetStudyIntegrityQueueDataSource"
				OnObjectDisposing="DisposeStudyIntegrityQueueDataSource"/>
				<ccUI:GridView ID="StudyIntegrityQueueGridView" runat="server"
					OnSelectedIndexChanged="StudyIntegrityQueueGridView_SelectedIndexChanged"
					OnPageIndexChanging="StudyIntegrityQueueGridView_PageIndexChanging"
					SelectionMode="Single" 
					OnRowDataBound="StudyIntegrityQueueItemList_RowDataBound">
					<Columns>
					    <asp:BoundField DataField="StudyInstanceUID" HeaderText="Study Instance UID" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
						</asp:BoundField>
						<asp:TemplateField HeaderText="Existing Study" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <asp:Label ID="ExistingPatientId" runat="server"></asp:Label> / <asp:Label ID="ExistingPatientName" runat="server"></asp:Label> / <asp:Label ID="ExistingAccessionNumber" runat="server"></asp:Label>
                            </itemtemplate>
						</asp:TemplateField>
		                <asp:TemplateField HeaderText="Conflicting Study" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                                <asp:Label ID="ConflictingPatientId" runat="server"></asp:Label> / <asp:Label ID="ConflictingPatientName" runat="server"></asp:Label> / <asp:Label ID="ConflictingAccessionNumber" runat="server"></asp:Label>
                            </itemtemplate>
						</asp:TemplateField>
			            <asp:TemplateField HeaderText="Time Received" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="TimeReceived" runat="server" Value='<%# Eval("ReceivedTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                        </asp:TemplateField>   
					</Columns>
				    <EmptyDataTemplate>
                        <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No items were found using the provided criteria." />
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
