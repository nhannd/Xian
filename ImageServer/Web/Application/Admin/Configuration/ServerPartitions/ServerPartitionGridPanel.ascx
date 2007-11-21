<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionGridPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionGridPanel" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
<asp:Panel ID="Panel1" runat="server" Width="100%" Height="480px" BorderColor="Silver" BorderWidth="1px">
<asp:GridView ID="PartitionGridView" runat="server" AutoGenerateColumns="False" 
    Width="100%" OnRowDataBound="PartitionGridView_RowDataBound" 
    AllowSorting="True" CellPadding="0" AllowPaging="True" CaptionAlign="Top" BorderWidth="2px">
    <Columns>
        <asp:BoundField DataField="AeTitle" HeaderText="AE Title" />
        <asp:BoundField DataField="Description" HeaderText="Description" />
        <asp:BoundField DataField="Port" HeaderText="Port" />
        <asp:BoundField DataField="PartitionFolder" HeaderText="Partition Folder" />
        <asp:TemplateField HeaderText="Enabled">
            <EditItemTemplate>
                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Enabled") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Image ID="ActiveImage" runat="server" />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <HeaderStyle HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
    
    <RowStyle CssClass="GridRowStyle"/>
    <EditRowStyle BackColor="#2461BF" Wrap="True" BorderColor="Yellow" />                
     <HeaderStyle CssClass="GridHeader" />
     <SelectedRowStyle CssClass="GridSelectedRowStyle" />
</asp:GridView>
</asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
