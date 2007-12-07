<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchGridView.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchGridView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="Panel1" runat="server">
    <asp:Panel ID="Panel2" runat="server" BorderColor="Silver" BorderWidth="1px" Height="400px">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ForeColor="#333333"
            OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
            OnSelectedIndexChanging="GridView1_SelectedIndexChanging" EmptyDataText="No studies available (Please check the filter settings!)"
            OnPageIndexChanging="GridView1_PageIndexChanging" PageSize="15" AllowSorting="True"
            Width="100%" CellPadding="0" AllowPaging="True" BackColor="Transparent" CaptionAlign="Top"
            BorderWidth="2px" CssClass="MyGrid">
            <FooterStyle BackColor="#507CD1" ForeColor="White" />
            <Columns>
                <asp:BoundField DataField="PatientName" HeaderText="Patient Name" />
                <asp:BoundField DataField="PatientId" HeaderText="Patient ID" />
                <asp:BoundField DataField="StudyDate" HeaderText="Study Date" />
                <asp:BoundField DataField="AccessionNumber" HeaderText="Accession Number" />
                <asp:BoundField DataField="StudyDescription" HeaderText="Description" />
            </Columns>
            <RowStyle CssClass="GridRowStyle" Height="24px" />
            <SelectedRowStyle CssClass="GridSelectedRowStyle" />
            <PagerStyle BackColor="Silver" HorizontalAlign="Center" CssClass="TableHeaderRow" />
            <HeaderStyle CssClass="GridHeader" Font-Bold="False" />
            <PagerTemplate>
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="PagerStudyCountLabel" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="PagerPagingLabel" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td align="right">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:ImageButton ID="PagerPrevImageButton" runat="server" CommandArgument="Prev"
                                            CommandName="Page" ImageUrl="~/images/prev.gif" OnCommand="ImageButton_Command" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="PagerNextImageButton" runat="server" CommandArgument="Next"
                                            CommandName="Page" ImageUrl="~/images/next.gif" OnCommand="ImageButton_Command" />
                                    </td>
                                </tr>
                            </table>
                    </tr>
                </table>
            </PagerTemplate>
        </asp:GridView>
    </asp:Panel>
</asp:Panel>
