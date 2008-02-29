<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.SeriesDetails.SeriesDetailsView" %>
<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    ForeColor="#333333" GridLines="Horizontal" BorderWidth="1px" Width="100%">
    <Fields>
        <asp:BoundField DataField="SeriesInstanceUid" HeaderText="Series Instance UID">
            
        </asp:BoundField>
        <asp:BoundField DataField="Modality" HeaderText="Modality">
            
        </asp:BoundField>
        <asp:BoundField DataField="SeriesNumber" HeaderText="Series Number">
            
        </asp:BoundField>
        
        <asp:BoundField DataField="SeriesDescription" HeaderText="Series Description">
            
        </asp:BoundField>
        
        
        <asp:BoundField DataField="NumberOfSeriesRelatedInstances"  HeaderText="Images">
            
        </asp:BoundField>
        <asp:BoundField DataField="PerformedDateTime"  HeaderText="Performed On">
            
        </asp:BoundField>
        <asp:BoundField DataField="SourceApplicationEntityTitle" HeaderText="Source AE">
            
        </asp:BoundField>
        
    </Fields>
    <RowStyle CssClass="CSSStudyDetailsViewRowStyle" BackColor="#F7F6F3" ForeColor="#333333" BorderStyle="solid"  BorderColor="AliceBlue" BorderWidth="2px" />
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
    <FieldHeaderStyle BackColor="#E9ECF1"  />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <HeaderStyle CssClass="CSSStudyDetailsViewHeaderStyle" />
    <EditRowStyle BackColor="#999999" />
    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    
</asp:DetailsView>
