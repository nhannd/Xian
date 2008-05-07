<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.StudyDetailsView" %>

<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
        <asp:BoundField DataField="StudyDescription" HeaderText="Study Description">
            <HeaderStyle Wrap="False" />
            
        </asp:BoundField>
        <asp:BoundField DataField="AccessionNumber" HeaderText="Accession Number">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
            
        <asp:TemplateField HeaderText="Referring Physician">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("ReferringPhysiciansName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="StudyInstanceUid" HeaderText="Study Instance UID">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Study Status">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Eval("StudyStatusEnum.Description") %>' />
            </ItemTemplate>
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Study Date/Time">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <ccUI:DALabel ID="StudyDate" runat="server"  Value='<%# Eval("StudyDate") %>' ></ccUI:DALabel>
                <ccUI:TMLabel ID="StudyTime" runat="server"  Value='<%# Eval("StudyTime") %>' ></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField>

        
        <asp:BoundField DataField="StudyID" HeaderText="Study ID">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="Series">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="Instances">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        
    </Fields>
    <RowStyle CssClass="CSSStudyDetailsViewRowStyle"/>
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
    <FieldHeaderStyle BackColor="#E9ECF1"  />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <HeaderStyle CssClass="CSSStudyDetailsViewHeaderStyle" Wrap="False" />
    <EditRowStyle BackColor="#999999" />
    <AlternatingRowStyle CssClass="CSSStudyDetailsViewAlternatingRowStyle" />
    
</asp:DetailsView>
