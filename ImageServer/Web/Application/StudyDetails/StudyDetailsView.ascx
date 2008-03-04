<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsView" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="cc1" %>
<asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
        <asp:BoundField DataField="StudyDescription" HeaderText="Study Description">
            <HeaderStyle Wrap="False" />
            
        </asp:BoundField>
        <asp:BoundField DataField="AccessionNumber" HeaderText="Accession Number">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="ReferringPhysiciansName" HeaderText="Referring Physician">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        
        <asp:BoundField DataField="StudyInstanceUid" HeaderText="Study Instance UID">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Study Status">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "StudyStatusEnum.Description") %>' />
            </ItemTemplate>
            <HeaderStyle Wrap="False" />
        </asp:TemplateField>
        
        <asp:BoundField DataField="StudyDate"  HeaderText="Study Date" Visible="False">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="StudyTime"  HeaderText="Study Time" Visible="False">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        
         <asp:TemplateField HeaderText="Study DateTime">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label runat="server" ID="StudyDateTimeLabel" />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField DataField="StudyID" HeaderText="Study ID">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedSeries" HeaderText="Series">
            <HeaderStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="NumberOfStudyRelatedInstances" HeaderText="Images">
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
