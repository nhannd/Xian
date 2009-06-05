<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationLogGridView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog.ApplicationLogGridView" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        <asp:ObjectDataSource ID="ApplicationLogDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.ApplicationLogDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Model.ApplicationLog" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetApplicationLogDataSource"
				OnObjectDisposing="DisposeApplicationLogDataSource"/>
			
            <ccUI:GridView ID="ApplicationLogListControl" runat="server"
				OnPageIndexChanging="ApplicationLogListControl_PageIndexChanging" 
				OnRowDataBound="GridView_RowDataBound"
				EmptyDataText="No logs found (Please check the filters.)">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Host" HeaderText="Host" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
					<asp:TemplateField HeaderText="Timestamp">
						<HeaderStyle Wrap="false" HorizontalAlign="Center" />
						<ItemStyle Wrap="false" HorizontalAlign="Center" />
						<ItemTemplate>
							<asp:Label ID="Timestamp" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("Timestamp"),DateTimeFormatter.Style.Timestamp) %>'/>
						</ItemTemplate>
					</asp:TemplateField>
                    <asp:BoundField DataField="Thread" HeaderText="Thread" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    	<ItemStyle Wrap="false" HorizontalAlign="Center" />
					</asp:BoundField>
                    <asp:BoundField DataField="LogLevel" HeaderText="Log Level" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
                    <asp:BoundField DataField="MessageException" HeaderText="Message" HeaderStyle-HorizontalAlign="Left" HtmlEncode="false"  />
                </Columns>
                <EmptyDataTemplate>
                   <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No log messages." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
