<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<%@ Register Src="SearchAccordian.ascx" TagName="SearchAccordian" TagPrefix="accordian" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="PagePanel" runat="server" CssClass="PagePanel">
            <asp:Table ID="Table" runat="server" Width="100%">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" Width="100%" />
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="PageFilterPanel">
                            <table cellpadding="3" cellspacing="0">
                                <tr>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label1" runat="server" Text="Patient Name" Style="padding-right: 5px"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="PatientName" runat="server" Width="100px" ToolTip="Filter the list by Patient Name" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label2" runat="server" Text="Patient ID" Style="padding-right: 5px"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="PatientId" runat="server" Width="100px" ToolTip="Filter the list by Patient Id" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label3" runat="server" Text="Accession#" Style="padding-right: 5px"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="AccessionNumber" runat="server" Width="100px" ToolTip="Filter the list by Accession Number" />
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label4" runat="server" Text="Description" Width="68px" Style="padding-right: 5px"
                                            EnableViewState="False" /><br />
                                        <asp:TextBox ID="StudyDescription" runat="server" Width="100px" ToolTip="Filter the list by Study Description" />
                                    </td>
                                    <td align="right" valign="bottom" style="width: 57px">
                                        <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                            OnClick="FilterButton_Click" ToolTip="Filter" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableFooterRow>
                    <asp:TableCell ColumnSpan="2">
                        <accordian:SearchAccordian ID="SearchAccordianControl" runat="server" />
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
