<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRuleGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRuleGridView" %>
    <asp:Panel ID="Panel1" runat="server" BorderColor="Silver" BorderWidth="1px" Height="400px">
        <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ForeColor="#333333"
            OnRowDataBound="GridView_RowDataBound" OnDataBound="GridView_DataBound"
            OnSelectedIndexChanged="GridView_SelectedIndexChanged"
            OnSelectedIndexChanging="GridView_SelectedIndexChanging"
            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" AllowSorting="True"
            EmptyDataText="No Server Rules available (Please check the filter settings!)"
            Width="100%" CellPadding="0" AllowPaging="True" BackColor="Transparent" CaptionAlign="Top"
            BorderWidth="2px" CssClass="MyGrid">
            <FooterStyle BackColor="#507CD1" ForeColor="White" />
            <Columns>
                <asp:BoundField DataField="RuleName" HeaderText="Rule Name"></asp:BoundField>
                <asp:TemplateField HeaderText="ServerRuleTypeEnum">
                    <ItemTemplate>
                        <asp:Label ID="ServerRuleTypeEnum" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ServerRuleApplyTimeEnum">
                    <ItemTemplate>
                        <asp:Label ID="ServerRuleApplyTimeEnum" runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Enabled">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Image ID="EnabledImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Default">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Default") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Image ID="DefaultImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <RowStyle CssClass="GridRowStyle" Height="24px" />
            <SelectedRowStyle CssClass="GridSelectedRowStyle" />
            <PagerStyle BackColor="Silver" HorizontalAlign="Center" CssClass="TableHeaderRow" />
            <HeaderStyle CssClass="GridHeader" Font-Bold="False" />
            <PagerTemplate>
            </PagerTemplate>
        </asp:GridView>
    </asp:Panel>
