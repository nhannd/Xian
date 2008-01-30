<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerPartitionGridPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionGridPanel" %>
<asp:Panel runat="server" CssClass="CSSGridViewPanelContainer">
    <asp:Panel runat="server" CssClass="CSSGridViewPanelBorder">
        <asp:Panel runat="server" CssClass="CSSGridViewPanelContent">
            <asp:GridView ID="PartitionGridView" runat="server" AutoGenerateColumns="False" CssClass="CSSGridView"
                Width="100%" OnRowDataBound="PartitionGridView_RowDataBound" AllowSorting="True"
                PageSize="20" CellPadding="0" CellSpacing="0" AllowPaging="True" 
                CaptionAlign="Top" BorderWidth="0px">
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

                </Columns>
                <emptydatatemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" CssClass="CSSGridHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>
                            AE Title
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Description
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Port
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Partition Folder
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center" >
                            Enabled
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center" >
                            Accept Any Device
                            </asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                   
                </emptydatatemplate> 
                <RowStyle CssClass="CSSGridRowStyle" />
                <HeaderStyle CssClass="CSSGridHeader" />
                <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
            </asp:GridView>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
