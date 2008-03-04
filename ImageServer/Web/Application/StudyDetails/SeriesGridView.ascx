<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.SeriesGridView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" 
    Assembly="ClearCanvas.ImageServer.Web.Common" %>
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" CssClass="CSSGridViewPanelContainer">
            <asp:Panel ID="Panel3" runat="server" CssClass="CSSGridViewPanelBorder" BorderWidth="0px">
                <asp:Panel ID="Panel4" runat="server" CssClass="CSSGridViewPanelContent" Height="50%">
                    <clearcanvas:GridView ID="GridView1" runat="server" 
                        AutoGenerateColumns="False" CssClass="CSSGridView" 
                        CellPadding="4" CaptionAlign="Top" Width="100%" 
                        OnPageIndexChanged="GridView1_PageIndexChanged" 
                        OnPageIndexChanging="GridView1_PageIndexChanging" SelectionMode="Multiple"
                        GridLines="Horizontal" BorderWidth="1px">
                        <Columns>
                            <asp:BoundField DataField="SeriesNumber" HeaderText="Series #">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="Modality" HeaderText="Modality">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesDescription" HeaderText="Description">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="NumberOfSeriesRelatedInstances" HeaderText="Images">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesInstanceUid" HeaderText="Series Instance UID">
                                <HeaderStyle Wrap="False" />  
                            </asp:BoundField>
                            <asp:TemplateField  HeaderText="Performed On">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="SeriesPerformedDateTime" Text="" Nowrap="true" />
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>
                             
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                                CssClass="CSSSeriesGridViewHeaderStyle">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Series#</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Modality</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Images</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Series Instance UID</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Performed On</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="CSSSeriesGridViewRowStyle"/>
                        <HeaderStyle CssClass="CSSSeriesGridViewHeaderStyle"/>
                        <AlternatingRowStyle CssClass="CSSSeriesGridViewAlternatingRowStyle" />
                        <SelectedRowStyle  CssClass="CSSSeriesGridSelectedRowStyle" />
                    </clearcanvas:GridView>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
