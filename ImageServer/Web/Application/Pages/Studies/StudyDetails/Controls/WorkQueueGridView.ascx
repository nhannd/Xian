<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.WorkQueueGridView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Common"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Data"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
	Width="100%">
	<asp:TableRow VerticalAlign="top">
		<asp:TableCell VerticalAlign="top">   
        <ccUI:GridView ID="StudyWorkQueueGridView" runat="server" 
                       OnPageIndexChanged="StudyWorkQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="StudyWorkQueueGridView_PageIndexChanging" SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="WorkQueueTypeEnum" HeaderText="Type">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Schedule">
                                <HeaderStyle wrap="False" />    
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="ScheduledTime" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="WorkQueuePriorityEnum" HeaderText="Priority">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Status">
                                <HeaderStyle wrap="False" />    
                                <ItemTemplate>
                                    <table>
                                    <tr>
                                    <td style="border-bottom:none"><%# Eval("WorkQueueStatusEnum")  %></td>
                                    <td style="border-bottom:none"><asp:Image runat="server" Visible='<%# !(Container.DataItem as WorkQueue).WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) && !ServerPlatform.IsActiveWorkQueue(Container.DataItem as WorkQueue) %>'  ImageAlign="AbsBottom" ID="StuckIcon" SkinID="WarningSmall" 
                                        ToolTip="There seems to be no activity for this item. The server may be down or there is a problem with this entry"/></td>
                                    </tr>
                                    </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                                
                            <asp:BoundField DataField="ProcessorID" HeaderText="Processing Server">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>
                            <asp:BoundField DataField="FailureDescription" HeaderText="Notes">
                                <HeaderStyle wrap="False" />  
                            </asp:BoundField>                            
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Priority</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Processing Server</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Notes</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="6" Height="50" HorizontalAlign="Center">
                                        <asp:panel runat="server" CssClass="GlobalGridViewEmptyText">No Work Queue items for this study.</asp:panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>   
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
                    
    </ContentTemplate>
</asp:UpdatePanel>
