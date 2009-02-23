<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TierMigrationWorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.TierMigrationWorkQueueDetailsView" %>

<asp:DetailsView ID="TierMigrationStudyDetailsView" runat="server" AutoGenerateRows="False" CellPadding="2"
    GridLines="Horizontal" CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Type" runat="server" Text='<%# Eval("Type.Description") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
           
        <asp:TemplateField HeaderText="Scheduled Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ScheduledDateTime" runat="server"  Value='<%# Eval("ScheduledDateTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expiration Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ExpirationTime" runat="server"  Value='<%# Eval("ExpirationTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Insert Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="InsertTime" runat="server"  Value='<%# Eval("InsertTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Status">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Status" runat="server" Text='<%# Eval("Status.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Priority">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Priority" runat="server" Text='<%# Eval("Priority.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Processing Server" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="ServerDescription" />
                
        <asp:TemplateField HeaderText="Patient Name">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("Study.PatientName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Patient ID">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# Eval("Study.PatientId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
       
        <asp:BoundField HeaderText="Failure Count" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="FailureCount" />
        <asp:BoundField HeaderText="Failure Description" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="FailureDescription" />
        <asp:BoundField HeaderText="Number of Instances Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumInstancesPending" />
        <asp:BoundField HeaderText="Number of Series Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumSeriesPending" />
        <asp:BoundField HeaderText="Current Filesystem Storage Location" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="StorageLocationPath" />
    </Fields>
    <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>



