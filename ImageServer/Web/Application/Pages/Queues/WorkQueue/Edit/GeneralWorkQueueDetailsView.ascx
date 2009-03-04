<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GeneralWorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.GeneralWorkQueueDetailsView" %>

    <asp:DetailsView ID="GeneralInfoDetailsView" runat="server" AutoGenerateRows="False" CellPadding="2" 
    GridLines="Horizontal" CssClass="GlobalGridView" Width="100%" OnDataBound="GeneralInfoDetailsView_DataBound">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <asp:Label ID="Type" runat="server" Text='<%# Eval("Type.Description") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:TemplateField>
        
         <asp:TemplateField HeaderText="Scheduled Date/Time">
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ScheduledDateTime" runat="server"  Value='<%# Eval("ScheduledDateTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Expiration Date/Time">
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="ExpirationTime" runat="server"  Value='<%# Eval("ExpirationTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Insert Date/Time">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <ccUI:DateTimeLabel ID="InsertTime" runat="server"  Value='<%# Eval("InsertTime") %>' ></ccUI:DateTimeLabel>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="Status" runat="server" Text='<%# Eval("Status.Description") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Priority">
            <ItemTemplate>
                <asp:Label ID="Priority" runat="server" Text='<%# Eval("Priority.Description") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Processing Server" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="ServerDescription" /> 
        
        <asp:TemplateField HeaderText="Patient Name">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:PersonNameLabel ID="ReferringPhysiciansName" runat="server" PersonName='<%# Eval("Study.PatientName") %>' PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Patient ID">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# Eval("Study.PatientId") %>' />
            </ItemTemplate>
        </asp:TemplateField>
       
        <asp:BoundField HeaderText="Failure Count" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="FailureCount" />
        <asp:TemplateField HeaderText="Failure Description">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label runat="server" ID="PatientID" Text='<%# HtmlUtility.Encode(Eval("FailureDescription").ToString()) %>' />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Number of Instances Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumInstancesPending" />
        <asp:BoundField HeaderText="Number of Series Pending" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="NumSeriesPending" />
        <asp:BoundField HeaderText="Filesystem Storage Location" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="StorageLocationPath" />

    </Fields>
    <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>

