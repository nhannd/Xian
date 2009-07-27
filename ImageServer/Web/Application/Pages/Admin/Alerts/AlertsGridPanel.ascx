<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Data.DataSource"%>
<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
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
            <ccUI:GridView ID="AlertGridView" runat="server" OnRowDataBound="AlertGridView_RowDataBound" SelectionMode="Multiple"
                DataKeyNames="Key">
                <Columns>
                    <asp:TemplateField HeaderText="Content">
					    <itemtemplate>
					        <%# Eval("Message") %>
					        <asp:LinkButton runat="server" ID="AppLogLink" Text="[Logs]" CssClass="LogInfo"/>
					        <asp:PlaceHolder runat="server" ID="DetailsHoverPlaceHolder"></asp:PlaceHolder>
					    </itemtemplate>
				    </asp:TemplateField>
                    <asp:BoundField DataField="Component" HeaderText="Component" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Source" HeaderText="Source" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="InsertTime" HeaderText="Insert&nbsp;Date" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:TemplateField HeaderText="Level" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
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
