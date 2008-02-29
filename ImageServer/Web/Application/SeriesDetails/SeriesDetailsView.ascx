<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.SeriesDetails.SeriesDetailsView" %>
<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Both" Width="100%">
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
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
    <RowStyle BackColor="#F7F6F3" ForeColor="red" />
    <FieldHeaderStyle  />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <HeaderStyle BackColor="#5D7B9D"  ForeColor="blue" />
    <EditRowStyle BackColor="#999999" />
    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    
</asp:DetailsView>
