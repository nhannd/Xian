<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeviceGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.Move.DeviceGridView" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound"
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnSelectedIndexChanging="GridView1_SelectedIndexChanging"
                EmptyDataText="" OnPageIndexChanging="GridView1_PageIndexChanging" CellPadding="0"
                PageSize="20" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="0px" BackColor="White" Height="100%">
                <Columns>
                    <asp:BoundField DataField="AETitle" HeaderText="AE Title" ></asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Description"></asp:BoundField>
                    <asp:TemplateField HeaderText="IPAddress">
                        <ItemTemplate>
                            <asp:Label ID="IpAddressLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Port" HeaderText="Port"></asp:BoundField>
                    <asp:TemplateField HeaderText="Enabled">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DHCP">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DHCPImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Partition" Visible="False">
                        <ItemTemplate>
                            <asp:Label ID="ServerParitionLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Features">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="FeaturePlaceHolder" runat="server"></asp:PlaceHolder>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="4" CellSpacing="0"
                        CssClass="GlobalGridViewHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>
                            AE Title
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Description
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            IPAddress
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Port
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">
                            Enabled
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">
                            DHCP
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">
                            Features
                            </asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow" />
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
            </asp:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
