<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SeriesGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.SeriesGridView" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
    
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
                    <ccUI:GridView ID="GridView1" runat="server" 
                        AutoGenerateColumns="False" CssClass="GlobalGridView" 
                        CellPadding="4" CaptionAlign="Top" Width="100%" 
                        OnPageIndexChanged="GridView1_PageIndexChanged" 
                        OnPageIndexChanging="GridView1_PageIndexChanging" SelectionMode="Multiple"
                        GridLines="Horizontal" BackColor="White">
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
                            <asp:BoundField DataField="NumberOfSeriesRelatedInstances" HeaderText="Instances">
                                <HeaderStyle Wrap="False" />    
                            </asp:BoundField>
                            <asp:BoundField DataField="SeriesInstanceUid" HeaderText="Series Instance UID">
                                <HeaderStyle Wrap="False" />  
                            </asp:BoundField>
                            <asp:TemplateField  HeaderText="Performed On">
                                <ItemTemplate>
                                    <ccUI:DALabel ID="PerformedDate" runat="server" Text="{0}" Value='<%# Eval("PerformedProcedureStepStartDate") %>' InvalidValueText="<i style='color:red'>[Invalid date:{0}]</i>"></ccUI:DALabel>
                                    <ccUI:TMLabel ID="PerformedTime" runat="server" Text="{0}" Value='<%# Eval("PerformedProcedureStepStartTime") %>' InvalidValueText="<i style='color:red'>[Invalid time:{0}]</i>"></ccUI:TMLabel>
                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>
                             
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell>Series#</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Modality</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Instances</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Series Instance UID</asp:TableHeaderCell>
                                    <asp:TableHeaderCell>Performed On</asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>
                    
    </ContentTemplate>
</asp:UpdatePanel>
