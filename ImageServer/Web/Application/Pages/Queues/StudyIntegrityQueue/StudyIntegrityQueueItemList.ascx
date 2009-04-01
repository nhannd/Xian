<%@ Import namespace="ClearCanvas.Common.Utilities"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.DataSource"%>
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
							    <table width="100%">
							    <tr>
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left">
							            <asp:Label runat="server" ID="ExistingPatientId" CssClass="StudyField" Text='<%# Eval("ExistingPatientId")%>'></asp:Label> / 
							            <asp:Label runat="server" ID="ExistingPatientName" CssClass="StudyField" Text='<%# Eval("ExistingPatientName")%>'></asp:Label>
							        </td>
							        
							    </tr>
							    <tr >
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left; font-size:80%;">
							            <asp:Label runat="server" ID="ExistingAccessionNumber" CssClass="StudyField" Text='<%# Eval("ExistingAccessionNumber")%>'></asp:Label> / 
                                        <asp:Label runat="server" ID="ExistingStudyDescription" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDescription")%>'></asp:Label> / 
							            <asp:Label runat="server" ID="ExistingStudyDate" CssClass="StudyField" Text='<%# Eval("StudySummary.StudyDate")%>'></asp:Label> / 
							            <asp:Label runat="server" ID="ExistingModalitiesInStudy" CssClass="StudyField" Text='<%# StringUtilities.EmptyIfNull(Eval("StudySummary.ModalitiesInStudy") as String).Replace("\\", ",") %>'></asp:Label>
							        </td>
							    </tr>
							    </table>                                
                            </itemtemplate>
						</asp:TemplateField>
		                <asp:TemplateField HeaderText="Conflicting Study" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
							<itemtemplate>
                               
                               <table width="100%">
							    <tr>
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left;">
							            <asp:Label runat="server" ID="ConflictingPatientId" CssClass="StudyField" Text='<%# Eval("QueueData.Details.StudyInfo.PatientInfo.PatientId")%>'></asp:Label> / 
							            <asp:Label runat="server" ID="ConflictingPatientName" CssClass="StudyField" Text='<%# Eval("QueueData.Details.StudyInfo.PatientInfo.Name")%>'></asp:Label>
							        </td>							        
							    </tr>
							    <tr >
							        <td style="border:none; vertical-align:top"></td>
							        <td style="border:none; vertical-align:top; text-align:left; font-size:80%;">
							            <asp:Label runat="server" ID="ConflictingAccessionNumber" CssClass="StudyField" Text='<%# Eval("QueueData.Details.StudyInfo.AccessionNumber")%>'></asp:Label> / <asp:Label runat="server" ID="ConflictingStudyDescription" CssClass="StudyField" Text='<%# Eval("QueueData.Details.StudyInfo.StudyDescription")%>'></asp:Label> / 
                                        <asp:Label runat="server" ID="ConflictingStudyDate" CssClass="StudyField"  Text='<%# Eval("QueueData.Details.StudyInfo.StudyDate")%>'></asp:Label> / 
                                        <asp:Label runat="server" ID="ConflictingModalities" CssClass="StudyField" Text='<%# StringUtilities.Combine(Eval("ConflictingModalities") as string[], ",") %>'></asp:Label>
							        </td>
							    </tr>
							    </table>    
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
					<RowStyle CssClass="GlobalGridViewRow StudyIntegrityQueueRow" />
					<AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow StudyIntegrityQueueRow" />
					<SelectedRowStyle CssClass="GlobalGridViewSelectedRow StudyIntegrityQueueRow" />
					<HeaderStyle CssClass="GlobalGridViewHeader" />
					<PagerTemplate>
					</PagerTemplate>
				</ccUI:GridView>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
