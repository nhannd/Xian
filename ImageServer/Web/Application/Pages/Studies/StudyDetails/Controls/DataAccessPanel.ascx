<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataAccessPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.DataAccessPanel" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="ClearCanvas.ImageServer.Model" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>

<div class="GridViewBorder">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Table runat="server" ID="ContainerTable" Height="100%" CellPadding="0" CellSpacing="0"
            Width="100%">
            <asp:TableRow VerticalAlign="top">
                <asp:TableCell VerticalAlign="top">
                    <ccUI:GridView ID="GridView1" runat="server" OnPageIndexChanged="GridView1_PageIndexChanged"
                        OnPageIndexChanging="GridView1_PageIndexChanging" MouseHoverRowHighlightEnabled="true"
                        RowHighlightColor="#EEEEEE" SelectionMode="Multiple" GridLines="Horizontal" BackColor="White">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="<%$Resources: ColumnHeaders, StudyDetails_GroupName%>">
                                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Description" HeaderText="<%$Resources: ColumnHeaders, AdminUserGroups_GroupDescription%>">
                                <HeaderStyle Wrap="False" HorizontalAlign="Left" />
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:BoundField>
                        </Columns>
                        <EmptyDataTemplate>
                            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0">
                                <asp:TableHeaderRow CssClass="GlobalGridViewHeader">
                                    <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupName%></asp:TableHeaderCell>
                                    <asp:TableHeaderCell><%=ColumnHeaders.AdminUserGroups_GroupDescription%></asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell ColumnSpan="5" Height="50" HorizontalAlign="Center">
                                        <asp:Panel ID="Panel1" runat="server" CssClass="GlobalGridViewEmptyText">
                                            <%= SR.StudyDetails_NoAuthorityGroupsForThisStudy%></asp:Panel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </EmptyDataTemplate>
                        <RowStyle CssClass="GlobalGridViewRow" />
                        <HeaderStyle CssClass="GlobalGridViewHeader" />
                        <AlternatingRowStyle CssClass="GlobalGridViewAlternatingRow" />
                        <SelectedRowStyle CssClass="GlobalGridViewSelectedRow" />
                    </ccUI:GridView>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ContentTemplate>
</asp:UpdatePanel>
</div>