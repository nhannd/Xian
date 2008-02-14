<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel" %>
<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="uc1" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" 
    Assembly="ClearCanvas.ImageServer.Web.Common" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="PagePanel" runat="server">
            <asp:Table ID="Table" runat="server"  
                CellPadding="0" CellSpacing="0"
                BorderWidth="0px" Width="100%">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="Left">
                        <asp:Panel ID="Panel1"  runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarContent">
                                    <clearcanvas:ToolbarButton 
                                        ID="DeleteToolbarButton" runat="server" 
                                        EnabledImageURL="~/images/icons/DeleteEnabled.png" 
                                        DisabledImageURL="~/images/icons/DeleteDisabled.png"
                                        AlternateText="Delete study"
                                        OnClick="OnDeleteToolbarButtonClick"
                                        />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom" >
                        <table cellpadding="0" cellspacing="0">
                            <!-- need this table so that the filter panel container is fit to the content -->
                            <tr>
                                <td>
                                <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel5" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server"  CssClass="CSSFilterPanelContent">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Patient Name" CssClass="CSSTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientName" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by Patient Name" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Patient ID" CssClass="CSSTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by Patient Id" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Accession#" CssClass="CSSTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="AccessionNumber" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by Accession Number" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Description" CssClass="CSSTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="StudyDescription" runat="server" CssClass="CSSFilterTextBox" ToolTip="Filter the list by Study Description" />
                                            </td>
                                            <td align="right" valign="bottom" style="width: 57px">
                                                <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                                    OnClick="FilterButton_Click" ToolTip="Filter" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <uc1:StudyListGridView id="StudyListGridView1" runat="server"></uc1:StudyListGridView>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <uc8:GridPager ID="GridPager1" runat="server"></uc8:GridPager>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
