<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Utilities"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkQueueSearchResultPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.WorkQueueSearchResultPanel" %>

<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
    
<asp:Panel ID="Panel1" runat="server" CssClass="CSSGridViewPanelContainer">
    <asp:Panel ID="Panel3" runat="server" CssClass="CSSGridViewPanelBorder">
        <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelContent">
            <clearcanvas:GridView ID="GridView1" runat="server" CssClass="CSSGridView" SelectionMode="Multiple" AutoGenerateColumns="false"
            PageSize="20" CellSpacing="0" CellPadding="0"  AllowPaging="True" CaptionAlign="Top" BorderWidth="0px" Width="100%" >
                <Columns>
                    <asp:BoundField  DataField="PatientID" HeaderText="Patient ID"></asp:BoundField>
                    <asp:TemplateField HeaderText="Patient Name">
                        <ItemTemplate>
                            <clearcanvas:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientName") %>' PersonNameType="Dicom"></clearcanvas:PersonNameLabel>                            
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <asp:Label ID="Type" runat="server" Text='<%# Eval("Type.Description") %>'></asp:Label> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Schedule">
                        <ItemTemplate>                            
                            <asp:Label id="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledDateTime")) %>'></asp:Label> 
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <asp:Label ID="Status" runat="server" Text='<%# Eval("Status.Description") %>'></asp:Label> 
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0" CssClass="CSSGridHeader">
                        <asp:TableHeaderRow>
                            <asp:TableHeaderCell>Patient ID</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Patient Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                            <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </EmptyDataTemplate> 
                <RowStyle CssClass="CSSGridRowStyle" />
                <AlternatingRowStyle CssClass="CSSGridAlternatingRowStyle" />
                <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
                <HeaderStyle CssClass="CSSGridHeader" />
            </clearcanvas:GridView>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
