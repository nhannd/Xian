<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArchivePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.ArchivePanel" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="StudyDetailsSubTitle">Archive Queue</div>
        
        <ccUI:GridView ID="ArchiveQueueGridView" runat="server" 
                       AutoGenerateColumns="False" CssClass="GlobalGridView" 
                       CellPadding="0" CaptionAlign="Top" Width="100%"
                       OnPageIndexChanged="ArchiveQueueGridView_PageIndexChanged" 
                       OnPageIndexChanging="ArchiveQueueGridView_PageIndexChanging" 
                       SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                        <Columns>
                            <asp:BoundField DataField="ArchiveQueueStatusEnum" HeaderText="Status">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="ScheduledTime" HeaderText="Schedule">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="ProcessorID" HeaderText="Processor ID">
                                <HeaderStyle wrap="False" />    
                            </asp:BoundField>                            
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Processor ID</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">No Archive Queue items for this study.</asp:panel>
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

<div class="StudyDetailsSubTitle" style="margin-top: 9px;">Archive Study Storage</div>
<asp:DetailsView ID="ArchiveStudyStorageDetailsView" runat="server" AutoGenerateRows="False" 
                OnDataBound="ArchiveStudyStorageDetailsView_DataBound"
                GridLines="Horizontal" CellPadding="4" CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:BoundField DataField="ServerTransferSyntaxKey" HeaderText="Transfer Syntax: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="ArchiveTime" HeaderText="Archive Time: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Archive XML: ">
            <ItemTemplate>
                <asp:Label ID="XmlText" runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Fields>
    <EmptyDataTemplate>
        <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
            <asp:TableRow>
                <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                    <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">No Archive Study Storage items for this study.</asp:panel>
                </asp:TableCell>
            </asp:TableRow>
       </asp:Table>
    </EmptyDataTemplate>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>
