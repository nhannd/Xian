<%@ Import namespace="System.ComponentModel"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsView" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="cc1" %>
<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Both" Width="100%" CssClass="CSSSeriesGridView" OnDataBound="DetailsView1_DataBound">
    <Fields>
        <asp:BoundField DataField="SeriesInstanceUid" HeaderText="Series Instance UID">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Modality" HeaderText="Modality">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesNumber" HeaderText="Series Number">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SeriesDescription" HeaderText="Series Description">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Performed Date/Time">
            <ItemTemplate>
                <cc1:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></cc1:DALabel>
                <cc1:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></cc1:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="NumberOfSeriesRelatedInstances"  HeaderText="Instances">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="SourceApplicationEntityTitle" HeaderText="Source AE">
            <HeaderStyle  Wrap="false" />
        </asp:BoundField>
        
        
    </Fields>
    <RowStyle CssClass="CSSStudyDetailsViewRowStyle"/>
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
    <FieldHeaderStyle BackColor="#E9ECF1"  />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <HeaderStyle CssClass="CSSStudyDetailsViewHeaderStyle" />
    <EditRowStyle BackColor="#999999" />
    <AlternatingRowStyle CssClass="CSSStudyDetailsViewAlternatingRowStyle" />
</asp:DetailsView>

