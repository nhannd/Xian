<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsGridView"
    Codebehind="FileSystemsGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel2" runat="server" CssClass="GridPanel">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ForeColor="#333333"
        OnRowDataBound="GridView1_RowDataBound" 
        OnDataBound="GridView1_DataBound" 
        OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
        OnSelectedIndexChanging="GridView1_SelectedIndexChanging" 
        OnPageIndexChanging="GridView1_PageIndexChanging" 
        EmptyDataText="No filesystems found (Please check the filters!)"
        PageSize="15" AllowSorting="True" AllowPaging="True" 
        CellPadding="0" CaptionAlign="Top" BorderWidth="2px" Width="100%">
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
        <RowStyle CssClass="GridRowStyle" />
        <SelectedRowStyle CssClass="GridSelectedRowStyle" />
        <HeaderStyle CssClass="GridHeader" />
    </asp:GridView>
</asp:Panel>
