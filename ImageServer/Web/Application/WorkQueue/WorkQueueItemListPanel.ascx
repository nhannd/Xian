<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.Utilities" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemListPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.WorkQueueItemListPanel" %>
    
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    
    <ContentTemplate>
    
    <asp:Table runat="server" ID="ListContainerTable" Height="100%" CellPadding="0" CellSpacing="0" Width="100%" >
        <asp:TableRow VerticalAlign="top">
            <asp:TableCell ID="ListContainerCell" CssClass="CSSGridViewPanelContent" VerticalAlign="top">
                <asp:GridView ID="WorkQueueListView" runat="server" CssClass="CSSGridView" AutoGenerateColumns="false"
                            PageSize="10" CellSpacing="0" CellPadding="0" AllowPaging="true" PagerSettings-Visible="false"
                            CaptionAlign="Top" BorderWidth="0px" Width="100%" 
                            OnRowDataBound="WorkQueueListView_RowDataBound" 
                            OnPageIndexChanged="WorkQueueListView_PageIndexChanged" 
                            OnDataBound="WorkQueueListView_DataBound"  Height="100%"
                            OnSelectedIndexChanged="WorkQueueListView_SelectedIndexChanged">
                            <Columns>
                                <asp:BoundField DataField="PatientID" HeaderText="Patient ID">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Left" />
                                    <ItemStyle Wrap="false" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Patient Name">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Left" />
                                    <ItemStyle Wrap="false" />
                                    <ItemTemplate>
                                        <clearcanvas:PersonNameLabel ID="PatientName" runat="server" PersonName='<%# Eval("PatientName") %>'
                                            PersonNameType="Dicom"></clearcanvas:PersonNameLabel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Type">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Left" />
                                    <ItemStyle Wrap="false" HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Type" runat="server" Text='<%# Eval("Type.Description") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Schedule">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Center"/>
                                    <ItemStyle Wrap="false" HorizontalAlign="Center"/>
                                    <ItemTemplate>
                                        <asp:Label ID="Schedule" runat="server" Text='<%# DateTimeFormatter.Format((DateTime)Eval("ScheduledDateTime")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Priority">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Center" />
                                    <ItemStyle Wrap="false" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Status" runat="server" Text='<%# Eval("Priority.Description") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Center" />
                                    <ItemStyle Wrap="false" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="Status" runat="server" Text='<%# Eval("Status.Description") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Notes">
                                    <HeaderStyle Wrap="false" HorizontalAlign="Left"/>
                                    <ItemStyle Wrap="false" HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Notes" runat="server" Text='<%# Eval("Notes") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                                    CssClass="CSSGridHeader">
                                    <asp:TableHeaderRow>
                                        <asp:TableHeaderCell>Patient ID</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Patient Name</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Type</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Schedule</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Priority</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Notes</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                </asp:Table>
                            </EmptyDataTemplate>
                            <RowStyle CssClass="CSSGridRowStyle" />
                            <AlternatingRowStyle CssClass="CSSGridAlternatingRowStyle" />
                            <SelectedRowStyle CssClass="CSSGridSelectedRowStyle" />
                            <HeaderStyle CssClass="CSSGridHeader" />
                        </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        
    </asp:Table>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel"  DynamicLayout="false">
        <ProgressTemplate>
            <asp:Panel ID="ProgressPanel" runat="server" HorizontalAlign="right">
                <asp:Image ID="UpdatingImage" runat="server" ImageUrl="~/images/ajax-loader.gif" />
            </asp:Panel>
        </ProgressTemplate>
    </asp:UpdateProgress>
    
    <asp:Timer ID="RefreshTimer" runat="server" OnTick="RefreshTimer_Tick" Interval="30000"></asp:Timer>
    
    </ContentTemplate>
</asp:UpdatePanel>


<clearcanvas:ConfirmationDialog runat="server" ID="ConfirmationDialog" />