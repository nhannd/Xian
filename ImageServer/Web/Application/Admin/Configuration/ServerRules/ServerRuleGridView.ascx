<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ServerRuleGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRuleGridView" %>

<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell CssClass="CSSGridViewPanelContent" VerticalAlign="top">
        
            <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" CssClass="CSSGridView"
                Width="100%" OnRowDataBound="GridView_RowDataBound" OnDataBound="GridView_DataBound"
                OnSelectedIndexChanged="GridView_SelectedIndexChanged"
                OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="20" AllowSorting="True"
                CellPadding="0" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="0px">
                <Columns>
                    <asp:BoundField DataField="RuleName" HeaderText="Name" HeaderStyle-HorizontalAlign="Left"></asp:BoundField>
                    <asp:TemplateField HeaderText="Type" HeaderStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="ServerRuleTypeEnum" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Apply Time" ItemStyle-HorizontalAlign="Center">
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
                        <HeaderStyle  HorizontalAlign="Center" />
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
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                        CssClass="CSSGridHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>
                            Name
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Type
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Apply Time
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                            Enabled
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell >
                            Default
                            </asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate>
                <RowStyle CssClass="CSSGridRowStyle" />
                <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
                <HeaderStyle CssClass="CSSGridHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </asp:GridView>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
