<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserGridPanel" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell>
        <asp:ObjectDataSource ID="UserDataSourceObject" runat="server" TypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserDataSource"
				DataObjectTypeName="ClearCanvas.ImageServer.Web.Common.Data.DataSource.UserRowData" EnablePaging="true"
				SelectMethod="Select" SelectCountMethod="SelectCount" OnObjectCreating="GetUserDataSource"
				OnObjectDisposing="DisposeUserDataSource"/>
            <ccUI:GridView ID="UserGridView" runat="server" OnRowDataBound="UserGridView_RowDataBound"
                OnSelectedIndexChanged="UserGridView_SelectedIndexChanged" SelectionMode="Single">
                <Columns>
                    <asp:BoundField DataField="UserName" HeaderText="User ID" HeaderStyle-HorizontalAlign="Left" >
                        <itemstyle width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DisplayName" HeaderText="Name" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="User Groups" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="UserGroupTextBox" runat="server" TextMode="multiline" rows="2" columns="35" CssClass="UserGroupTextArea" ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Enabled" HeaderText="Enabled" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="LastLoginTime" HeaderText="Last Login" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" >
                        <itemstyle width="175px" />
                    </asp:BoundField>
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
