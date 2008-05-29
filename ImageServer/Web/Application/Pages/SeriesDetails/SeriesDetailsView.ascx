<%@ Import namespace="System.ComponentModel"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsView" %>

<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="2" 
    GridLines="Horizontal" Width="100%" CssClass="GlobalGridView" OnDataBound="DetailsView1_DataBound">
    <Fields>
        <asp:BoundField DataField="SeriesInstanceUid" HeaderText="Series Instance UID: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Modality" HeaderText="Modality: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesNumber" HeaderText="Series Number: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesDescription" HeaderText="Series Description: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Performed Date/Time: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></ccUI:DALabel>
                <ccUI:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="NumberOfSeriesRelatedInstances"  HeaderText="Instances: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SourceApplicationEntityTitle" HeaderText="Source AE: ">
            <HeaderStyle CssClass="SeriesDetailsGridViewHeader" Wrap="false" />
        </asp:BoundField>
        
        
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
</asp:DetailsView>

