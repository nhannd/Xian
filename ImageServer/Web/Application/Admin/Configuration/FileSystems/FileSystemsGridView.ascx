<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsGridView"
    Codebehind="FileSystemsGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server" CssClass="CSSGridViewPanelContainer">
    <asp:Panel ID="Panel3" runat="server" CssClass="CSSGridViewPanelBorder">
        <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelContent">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="CSSGridView"
                Width="100%" OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound"
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnSelectedIndexChanging="GridView1_SelectedIndexChanging"
                OnPageIndexChanging="GridView1_PageIndexChanging" EmptyDataText="No filesystems found (Please check the filters!)"
                PageSize="20" CellPadding="0" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
                BorderWidth="0px">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="Description" HeaderText="Description"></asp:BoundField>
                    <asp:TemplateField HeaderText="Read">
                        <ItemTemplate>
                            <asp:Image ID="ReadImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Write">
                        <ItemTemplate>
                            <asp:Image ID="WriteImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tier">
                        <ItemTemplate>
                            <asp:Label ID="FilesystemTierDescription" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Path">
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
                
                <emptydatatemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" CssClass="CSSGridHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>
                            Description
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center" >
                            Read
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="Center" >
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
                </emptydatatemplate> 
                
                <RowStyle CssClass="CSSGridRowStyle" />
                <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
                <HeaderStyle CssClass="CSSGridHeader" />
            </asp:GridView>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
