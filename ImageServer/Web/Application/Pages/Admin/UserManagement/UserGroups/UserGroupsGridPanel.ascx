<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGroupsGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserGroupDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserGroupDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserGroupRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserGroupDataSource"
				OnObjectDisposing="DisposeUserGroupsDataSource"/>
            <ccUI:GridView ID="UserGroupsGridView" runat="server" OnRowDataBound="UserGroupsGridView_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Group Name" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Tokens" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:TextBox ID="TokensTextBox" runat="server" TextMode="multiline" rows="3" columns="100" CssClass="TokenTextArea" ></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>                    
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No User Groups were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
