<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.FileSystems.FileSystemsGridView"
    Codebehind="FileSystemsGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound"
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnSelectedIndexChanging="GridView1_SelectedIndexChanging"
                OnPageIndexChanging="GridView1_PageIndexChanging" EmptyDataText="No filesystems found (Please check the filters!)"
                PageSize="20" CellPadding="0" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
                BorderWidth="0px">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Description" HeaderText="Description" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:TemplateField HeaderText="Read">
                        <ItemTemplate>
                            <asp:Image ID="ReadImage" runat="server" SkinID="Checked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Write">
                        <ItemTemplate>
                            <asp:Image ID="WriteImage" runat="server" SkinID="Unchecked" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tier" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="FilesystemTierDescription" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Path" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="PathLabel" runat="server" Text='<%# Bind("FileSystemPath") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HighWatermark" HeaderText="High Watermark" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LowWatermark" HeaderText="Low Watermark" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PercentFull" HeaderText="Percent Full" Visible="False">
                        <HeaderStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Disk Usage">
                        <ItemTemplate>
                            <asp:Image ID="UsageImage" runat="server" ImageAlign="AbsBottom" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" Wrap="False" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                        CssClass="GlobalGridViewHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>
                            Description
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">
                            Read
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center">
                            Write
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Tier
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Path
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Disk Usage
                            </asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
            </asp:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
