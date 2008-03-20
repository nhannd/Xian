
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks.ServiceLockGridView"
    Codebehind="ServiceLockGridView.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell CssClass="CSSGridViewPanelContent" VerticalAlign="top">
        
            <asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" CssClass="CSSGridView"
                Width="100%" OnRowDataBound="GridView_RowDataBound" OnDataBound="GridView_DataBound"
                OnSelectedIndexChanged="GridView_SelectedIndexChanged"
                EmptyDataText="" OnPageIndexChanging="GridView_PageIndexChanging" CellPadding="0" PagerSettings-Visible="false"
                PageSize="20" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="0px">
                <Columns>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                           <asp:Label ID="Type" runat="server" Text='<%# Eval("ServiceLockTypeEnum.Description") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                           <asp:Label ID="Description" runat="server" Text='<%# Eval("ServiceLockTypeEnum.LongDescription") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Enabled">
                        <ItemTemplate>
                            <asp:Image ID="EnabledImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Locked">
                        <ItemTemplate>
                            <asp:Image ID="LockedImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    
                     <asp:TemplateField HeaderText="Schedule">
                        <ItemTemplate>
                            <clearcanvas:DateTimeLabel ID="Schedule" runat="server" Text='<%# Eval("ScheduledTime") %>'></clearcanvas:DateTimeLabel>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Processor ID">
                        <ItemTemplate>
                            <asp:Label ID="ProcessorID" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    
                </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                        CssClass="CSSGridHeader">
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
                            <asp:TableHeaderCell>
                            Features
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
</ContentTemplate>
</asp:UpdatePanel>
