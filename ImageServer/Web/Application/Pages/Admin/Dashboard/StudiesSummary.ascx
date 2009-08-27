<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudiesSummary.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Dashboard.StudiesSummary" %>
<asp:DataList ID="StudiesDataList" runat="server" Width="100%" OnItemDataBound="Item_DataBound">
    <HeaderTemplate>
        <tr class="OverviewHeader"><td style="padding-left: 4px;">Partition</td><td align="center" nowrap="nowrap">Number of Studies</td></tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr><td style="padding: 2px 2px 2px 4px;" nowrap="nowrap"><asp:LinkButton runat="server" ID="PartitionLink"><%#Eval("AETitle") %></asp:LinkButton></td><td align="center" style="padding: 2px;"><%#Eval("StudyCount") %></td></tr>
    </ItemTemplate>
    <AlternatingItemStyle CssClass="OverviewAlernateRow" />
</asp:DataList>
<div class="TotalStudiesSummary">Total Studies: <asp:Label ID="TotalStudiesLabel" runat="server" Text="100,000,000" CssClass="TotalStudiesCount"/></div>