<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionGridPanel" %>
<asp:Panel ID="Panel1" runat="server" CssClass="GridPanel">
    <asp:GridView ID="PartitionGridView" runat="server" AutoGenerateColumns="False" Width="100%"
        OnRowDataBound="PartitionGridView_RowDataBound" AllowSorting="True" CellPadding="0"
        AllowPaging="True" CaptionAlign="Top" BorderWidth="2px">
        <Columns>
            <asp:BoundField DataField="AeTitle" HeaderText="AE Title" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:BoundField DataField="Port" HeaderText="Port" />
            <asp:BoundField DataField="PartitionFolder" HeaderText="Partition Folder" />
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
        </Columns>
        <RowStyle CssClass="GridRowStyle" />
        <HeaderStyle CssClass="GridHeader" />
        <SelectedRowStyle CssClass="GridSelectedRowStyle" />
    </asp:GridView>
</asp:Panel>
