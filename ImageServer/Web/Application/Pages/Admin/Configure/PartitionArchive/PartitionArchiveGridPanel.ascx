<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PartitionArchiveGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.PartitionArchive.PartitionArchiveGridPanel" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>       
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">             
            <ccUI:GridView ID="PartitionGridView" runat="server" 
                OnRowDataBound="PartitionGridView_RowDataBound"
                SelectionMode="Single">
                <Columns>
                                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left" />
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                             <asp:Label ID="ArchiveType" runat="server" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="ArchiveDelayHours" HeaderText="Archive Delay Hours">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Enabled">
                        <ItemTemplate>
                             <asp:Image ID="EnabledImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Read Only">
                        <ItemTemplate>
                             <asp:Image ID="ReadOnlyImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Configuration XML">
                         <ItemTemplate>
                             <asp:Label ID="ConfigurationXML" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                  <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="No partition archives were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <AlternatingRowStyle CssClass="GlobalGridViewRow" />                
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
            </ccUI:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</ContentTemplate>
</asp:UpdatePanel>