<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.WorkQueueSummary" %>
<asp:DataList ID="WorkQueueDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;">Server</td><td align="center" nowrap="nowrap">Number of Items</td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="WorkQueueLink"><%#Eval("Server") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("ItemCount") %></td></tr>
    </ItemTemplate>
</asp:DataList>