<%@ Import namespace="Microsoft.JScript"%>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesGridView.ascx.cs" 
Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.SeriesGridView" %>

            <asp:GridView ID="SeriesListControl" runat="server" AutoGenerateColumns="False"
                CssClass="GlobalGridView" Width="100%" EmptyDataText="" CellPadding="0" 
                PageSize="25" CellSpacing="0" AllowPaging="True" CaptionAlign="Top"
                BorderWidth="0px">
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

                                </ItemTemplate>
                                <HeaderStyle Wrap="False" />  
                            </asp:TemplateField>
                                    </Columns>
                <EmptyDataTemplate>
                    <ccAsp:EmptySearchResultsMessage ID="EmptySearchResultsMessage1" runat="server" Message="No series items for this study." />
                </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow"/>
                        <HeaderStyle CssClass="GlobalGridViewHeader"/>
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle  CssClass="GlobalGridSelectedRow" />
                <PagerTemplate>
                </PagerTemplate>
             </asp:GridView>

