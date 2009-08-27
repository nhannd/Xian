<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoRouteWorkQueueDetailsView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.AutoRouteWorkQueueDetailsView" %>

<ccAsp:SectionPanel ID="AutoRouteInfoSectionPanel" runat="server" HeadingText="Auto-Route" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="AutoRouteDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4"
    GridLines="Horizontal" CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:TemplateField HeaderText="Type">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Type" runat="server"  Text='<%# Eval("Type.Description") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Destination">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="Destination" runat="server" Text='<%# Eval("DestinationAE") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>  
        <asp:TemplateField HeaderText="Study Instance UID">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="False" />
            <ItemTemplate>
                <asp:Label ID="StudyInstanceUid" runat="server" Text='<%# Eval("StudyInstanceUid") %>' ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>      
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>

<ccAsp:SectionPanel ID="GeneralInfoSectionPanel" runat="server" HeadingText="General Information" HeadingCSS="CSSDefaultSectionHeading">
<SectionContentTemplate>
    <asp:DetailsView ID="GeneralInfoDetailsView" runat="server" AutoGenerateRows="False" CellPadding="4"
    GridLines="Horizontal" CssClass="GlobalGridView" Width="100%">
    <Fields>
    
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

        <asp:TemplateField HeaderText="Accession Number">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                 <asp:Label runat="server" ID="AccessionNumber" Text='<%# Eval("Study.AccessionNumber") %>' />
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Study Date/Time: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <ccUI:DALabel ID="StudyDate" runat="server"  Value='<%# Eval("Study.StudyDate") %>' ></ccUI:DALabel>
                <ccUI:TMLabel ID="StudyTime" runat="server"  Value='<%# Eval("Study.StudyTime") %>' ></ccUI:TMLabel>
            </ItemTemplate>
        </asp:TemplateField> 
        
        <asp:TemplateField HeaderText="Modalities">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <asp:Label runat="server" ID="Modalities" Text='<%# Eval("Study.Modalities") %>' />
            </ItemTemplate>
        </asp:TemplateField>
                
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
        <asp:BoundField HeaderText="Filesystem Storage Location" HeaderStyle-CssClass="StudyDetailsViewHeader" DataField="StorageLocationPath" />
        
    </Fields>
    <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
</asp:DetailsView>
</SectionContentTemplate>
</ccAsp:SectionPanel>



