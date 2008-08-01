<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudyStorageView.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls.StudyStorageView" %>

<asp:DetailsView ID="StudyStorageViewControl" runat="server" AutoGenerateRows="False" GridLines="Horizontal" CellPadding="4" OnDataBound="StudyStorageView_DataBound"
     CssClass="GlobalGridView" Width="100%">
    <Fields>
        <asp:BoundField DataField="InsertTime" HeaderText="Insert Time: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="LastAccessedTime" HeaderText="Last Accessed: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:BoundField DataField="Lock" HeaderText="Lock: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Status: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <asp:Label ID="Status" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Transfer Syntax: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <asp:Label ID="TransferSyntaxUID" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Study Folder: ">
            <HeaderStyle CssClass="StudyDetailsViewHeader" Wrap="false" />
            <ItemTemplate>
                <asp:Label ID="StudyFolder" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Fields>
    <RowStyle CssClass="GlobalGridViewRow"/>
    <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
    
</asp:DetailsView>
