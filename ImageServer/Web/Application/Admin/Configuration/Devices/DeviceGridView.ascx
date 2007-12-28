<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DeviceGridView"
    Codebehind="DeviceGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server">
    <asp:Panel ID="Panel2" runat="server" CssClass="GridPanel">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ForeColor="#333333"
            OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
            OnSelectedIndexChanging="GridView1_SelectedIndexChanging" EmptyDataText="No devices available (Please check the filter settings!)"
            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" AllowSorting="True"
            Width="100%" CellPadding="0" AllowPaging="True" BackColor="Transparent" CaptionAlign="Top"
            BorderWidth="2px">
            <FooterStyle BackColor="#507CD1" ForeColor="White" />
            <Columns>
                <asp:BoundField DataField="AETitle" HeaderText="AE Title"></asp:BoundField>
                <asp:BoundField DataField="Description" HeaderText="Description"></asp:BoundField>
                <asp:BoundField DataField="IPAddress" HeaderText="IP Address"></asp:BoundField>
                <asp:BoundField DataField="Port" HeaderText="Port"></asp:BoundField>
                <asp:TemplateField HeaderText="Enabled">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Image ID="ActiveImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="DHCP">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Image ID="DHCPImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
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
                </asp:TemplateField>
            </Columns>
            <RowStyle CssClass="GridRowStyle"/>
            <SelectedRowStyle CssClass="GridSelectedRowStyle" />
            <HeaderStyle CssClass="GridHeader" Font-Bold="False" />
            <PagerTemplate>
            </PagerTemplate>
        </asp:GridView>
    </asp:Panel>
</asp:Panel>
