<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.WorkQueueDetailsView" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
<asp:DetailsView ID="WorkQueueItemDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="80%">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Type" runat="server"  Text='<%# Eval("Type.Description") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
         <asp:TemplateField HeaderText="Scheduled Date/Time">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <clearcanvas:DateTimeLabel ID="ScheduledDateTime" runat="server"  Value='<%# Eval("ScheduledDateTime") %>' ></clearcanvas:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expiration Date/Time">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <clearcanvas:DateTimeLabel ID="ExpirationTime" runat="server"  Value='<%# Eval("ExpirationTime") %>' ></clearcanvas:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Status">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Status" runat="server"  Text='<%# Eval("Status.Description") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Priority">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Priority" runat="server"  Text='<%# Eval("Priority.Description") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Processor ID" DataField="ProcessorID" />
        
        
        <asp:TemplateField HeaderText="Patient Name">
            <ItemTemplate>
                <clearcanvas:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("Study.PatientName") %>' PersonNameType="Dicom"></clearcanvas:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Patient ID">
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# Eval("Study.PatientID") %>' />
            </ItemTemplate>
        </asp:TemplateField>
       
        <asp:BoundField HeaderText="Failure Count" DataField="FailureCount" />
        
        
        
        <asp:BoundField HeaderText="Number of Instances Pending" DataField="NumInstancesPending" />
        <asp:BoundField HeaderText="Number of Series Pending" DataField="NumSeriesPending" />
        
        
       
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
