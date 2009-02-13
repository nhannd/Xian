<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Users.UserGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserDataSource"
				OnObjectDisposing="DisposeUserDataSource"/>
            <ccUI:GridView ID="UserGridView" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
                Width="100%" AllowSorting="True" MouseHoverRowHighlightEnabled="true" RowHighlightColor="#eeeeee"
                PageSize="20" CellPadding="0" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" 
                BorderWidth="0px" HorizontalAlign="left" DataSourceID="UserDataSourceObject" OnRowDataBound="UserGridView_RowDataBound"
                OnSelectedIndexChanged="UserGridView_SelectedIndexChanged">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="Username" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="DisplayName" HeaderText="Display Name" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Enabled" HeaderText="Enabled" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="LastLoginTime" HeaderText="Last Login" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />                                                                        
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No users were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
