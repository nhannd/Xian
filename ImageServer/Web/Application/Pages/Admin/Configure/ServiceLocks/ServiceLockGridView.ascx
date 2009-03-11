<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServiceLocks.ServiceLockGridView"
    Codebehind="ServiceLockGridView.ascx.cs" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
    Width="100%">
    <asp:TableRow VerticalAlign="top">
        <asp:TableCell VerticalAlign="top">
        
            <ccUI:GridView ID="GridView" runat="server" AutoGenerateColumns="False" CssClass="GlobalGridView"
                Width="100%" OnRowDataBound="GridView_RowDataBound" OnDataBound="GridView_DataBound"
                OnSelectedIndexChanged="GridView_SelectedIndexChanged"
                EmptyDataText="" OnPageIndexChanging="GridView_PageIndexChanging" CellPadding="0" PagerSettings-Visible="false"
                PageSize="20" CellSpacing="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="0px">
                <Columns>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                           <asp:Label ID="Type" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                           <asp:Label ID="Description" runat="server"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Enabled">
                        <ItemTemplate>
                            <asp:Image ID="EnabledImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Locked">
                        <ItemTemplate>
                            <asp:Image ID="LockedImage" runat="server" SkinId="Unchecked" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="Schedule">
                        <ItemTemplate>
                            <ccUI:DateTimeLabel ID="Schedule" runat="server" Value='<%# Eval("ScheduledTime") %>' ></ccUI:DateTimeLabel>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" wrap="false" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Filesystem">
                        <ItemTemplate>
                            <asp:Label ID="Filesystem" runat="server" Text=""/>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle  />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Processor ID">
                        <ItemTemplate>
                            <asp:Label ID="ProcessorID" runat="server" Text='<%# Eval("ProcessorID") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" Wrap="false"/>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage" runat="server" Message="No services were found using the provided criteria." />
                </EmptyDataTemplate>
                <RowStyle CssClass="GlobalGridViewRow" />
                <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                <HeaderStyle CssClass="GlobalGridViewHeader" />
                <PagerTemplate>
                </PagerTemplate>
            </ccUI:GridView>

        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</ContentTemplate>
</asp:UpdatePanel>
