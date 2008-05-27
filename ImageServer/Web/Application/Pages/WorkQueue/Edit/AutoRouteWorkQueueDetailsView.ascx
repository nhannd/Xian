<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoRouteWorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.AutoRouteWorkQueueDetailsView" %>

<ccAsp:SectionPanel ID="AutoRouteInfoSectionPanel" runat="server" HeadingText="Auto-Route" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="AutoRouteDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Type" runat="server"  Text='<%# Eval("Type.Description") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Destination">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Destination" runat="server" Text='<%# Eval("DestinationAE") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>  
        <asp:TemplateField HeaderText="Study Instance UID">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="StudyInstanceUid" runat="server" Text='<%# Eval("StudyInstanceUid") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>      
    </Fields>
    <RowStyle CssClass="CSSStudyDetailsViewRowStyle"/>
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
    <FieldHeaderStyle BackColor="#E9ECF1"  />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <HeaderStyle CssClass="CSSStudyDetailsViewHeaderStyle" Wrap="False" />
    <EditRowStyle BackColor="#999999" />
    <AlternatingRowStyle CssClass="CSSStudyDetailsViewAlternatingRowStyle" />
    <FieldHeaderStyle Width="30%" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>

<ccAsp:SectionPanel ID="GeneralInfoSectionPanel" runat="server" HeadingText="General Information" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="GeneralInfoDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4" 
    GridLines="Horizontal" CssClass="CSSStudyDetailsView" Width="100%">
    <Fields>
    
        <asp:TemplateField HeaderText="Scheduled Date/Time">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ScheduledDateTime" runat="server"  Value='<%# Eval("ScheduledDateTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expiration Date/Time">
            <HeaderStyle Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ExpirationTime" runat="server"  Value='<%# Eval("ExpirationTime") %>' ></ccUI:DateTimeLabel>
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
        
        <asp:BoundField HeaderText="Processing Server" DataField="ServerDescription" />
        
        
        <asp:TemplateField HeaderText="Patient Name">
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("Study.PatientName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Patient ID">
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# Eval("Study.PatientId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
       
        <asp:BoundField HeaderText="Failure Count" DataField="FailureCount" />
        <asp:BoundField HeaderText="Failure Description" DataField="FailureDescription" />
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
    <FieldHeaderStyle Width="30%" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>



