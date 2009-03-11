<%@ Import namespace="System.Xml"%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="AlertsGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">   
            <asp:ObjectDataSource ID="AlertDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.AlertDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.AlertSummary" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetAlertDataSource"
				OnObjectDisposing="DisposeAlertDataSource"/>
            <ccUI:GridView ID="AlertGridView" runat="server" OnRowDataBound="AlertGridView_RowDataBound" SelectionMode="Single"
                DataKeyNames="Key" OnSelectedIndexChanged="AlertGridView_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="Content" HeaderText="Content" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Component" HeaderText="Component" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Source" HeaderText="Source" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="InsertTime" HeaderText="Insert Date" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Level">
					    <itemtemplate>
                            <asp:Label ID="Level" Text="" runat="server" />
                        </itemtemplate>
				    </asp:TemplateField>
                    <asp:BoundField DataField="Category" HeaderText="Category" HeaderStyle-HorizontalAlign="Left" />                                                                               
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No alerts were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
