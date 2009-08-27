<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.ServerPartitionGridView" %>


<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">   
            <ccUI:GridView ID="PartitionGridView" runat="server" 
                OnRowDataBound="PartitionGridView_RowDataBound" 
                PageSize="20" SelectionMode="Disabled" MouseHoverRowHighlightEnabled="false">
                <Columns>
                    <asp:BoundField DataField="AeTitle" HeaderText="AE Title" HeaderStyle-HorizontalAlign="Left"/>
                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="Port" HeaderText="Port" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="PartitionFolder" HeaderText="Partition Folder" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Enabled">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Accept Any Device">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("AcceptAnyDeviceImage") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="AcceptAnyDeviceImage" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Duplicate Object Policy" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="DuplicateSopDescription" runat="server"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Studies">
                        <ItemTemplate>
                            <asp:Label ID="Studies" runat="server" Text='<%# Bind("StudyCount") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No server partitions were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                                
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
