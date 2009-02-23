<%@ Import namespace="System.Xml"%>
<%@ Import namespace="ClearCanvas.ImageServer.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HistoryPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.HistoryPanel" %>
<%@ Register Src="StudyHistoryChangeDescPanel.ascx" TagName="StudyHistoryChangeDescPanel" TagPrefix="localAsp" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <ccUI:GridView ID="StudyHistoryGridView" runat="server" 
                       AutoGenerateColumns="false" CssClass="GlobalGridView" 
                       CellPadding="0" CaptionAlign="Top" Width="100%"
                       OnRowDataBound="StudyHistoryGridView_RowDataBound"
                       OnPageIndexChanged="StudyHistoryGridView_PageIndexChanged" 
                       OnPageIndexChanging="StudyHistoryGridView_PageIndexChanging" 
                       SelectionMode="Disabled"
                       MouseHoverRowHighlightEnabled="false"
                       GridLines="Horizontal" BackColor="White" >
                       <Columns>
                          <asp:TemplateField HeaderText="Timestamp">
                            <ItemTemplate>
                                <ccUI:DateTimeLabel ID="InsertTime" runat="server" Value='<%# Eval("InsertTime") %>' ></ccUI:DateTimeLabel>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="Description" runat="server" Text='<%# Eval("StudyHistoryTypeEnum") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Details">
                            <ItemTemplate>
                                <localAsp:StudyHistoryChangeDescPanel runat="server" id="StudyHistoryChangeDescPanel"></localAsp:StudyHistoryChangeDescPanel>
                            </ItemTemplate>
                        </asp:TemplateField>
                                                  
                       </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" >
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell>Timestamp</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Description</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Details</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="3" Height="50" HorizontalAlign="Center">
                                        <asp:panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">No history record is found for this study.</asp:panel>
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
