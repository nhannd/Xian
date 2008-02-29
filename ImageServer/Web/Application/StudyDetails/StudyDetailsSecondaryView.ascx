<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsSecondaryView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsSecondaryView" %>
<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    ForeColor="#333333" GridLines="Horizontal" BorderWidth="1px" Width="100%">
    <Fields>
        <asp:BoundField DataField="StudyDescription" Visible="false" HeaderText="Study Description">
            
        </asp:BoundField>
        <asp:BoundField DataField="AccessionNumber" Visible="false" HeaderText="Accession Number">
            
        </asp:BoundField>
        <asp:BoundField DataField="ReferringPhysiciansName" Visible="false" HeaderText="Referring Physician">
            
        </asp:BoundField>
        
        <asp:BoundField DataField="StudyInstanceUid"  HeaderText="Study Instance UID">
            
        </asp:BoundField>
        <asp:TemplateField HeaderText="StudyStatus" Visible="false" >
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "StudyStatusEnum.Description") %>' />
            </ItemTemplate>
            
        </asp:TemplateField>
        
        <asp:BoundField DataField="StudyDate" Visible="false"  HeaderText="Study Date">
            
        </asp:BoundField>
        <asp:BoundField DataField="StudyTime"  HeaderText="Study Time">
            
        </asp:BoundField>
        <asp:BoundField DataField="StudyID" HeaderText="Study ID">
            
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="Series">
            
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="Images">
            
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
