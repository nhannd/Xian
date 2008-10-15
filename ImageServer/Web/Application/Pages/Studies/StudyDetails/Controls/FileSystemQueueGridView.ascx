<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FileSystemQueueGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.FileSystemQueueGridView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <ccUI:GridView ID="FSQueueGridView" runat="server" 
                       AutoGenerateColumns="False" CssClass="GlobalGridView" 
                       CellPadding="0" CaptionAlign="Top" Width="100%"
                       OnPageIndexChanged="FSQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="FSQueueGridView_PageIndexChanging" SelectionMode="Disabled"
                       OnRowDataBound="FSQueueGridView_RowDataBound"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="FilesystemQueueTypeEnum" HeaderText="Type">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Scheduled Time">
                                <ItemTemplate>
                                    <ccUI:DateTimeLabel ID="ScheduledTime" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                                </ItemTemplate>
                                <HeaderStyle wrap="False" />    
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Queue XML">
                                <ItemTemplate>
                                    <asp:Label ID="XmlText" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Queue XML</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">No File System Queue items for this study.</asp:panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>                    
    </ContentTemplate>
</asp:UpdatePanel>
