<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Codebehind="ServerRulePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRulePage"
    Title="ImageServer Rules Engine" validateRequest="false"%>

<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs"
    TagPrefix="partitionTabs" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="confirm" %>
<%@ Register Src="AddEditServerRuleDialog.ascx" TagName="AddEditServerRuleDialog"
    TagPrefix="addedit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="ServerRulePagePanel" runat="server">
        <asp:Table ID="MainTable" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
            <asp:TableHeaderRow>
                <asp:TableHeaderCell CssClass="WindowTitleBar">
                Rules Engine Management
            </asp:TableHeaderCell>
            </asp:TableHeaderRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Panel ID="PageContent" runat="server" CssClass="ContentWindow">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <partitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                            <confirm:ConfirmDialog ID="ConfirmDialog" runat="server" />
                            <addedit:AddEditServerRuleDialog ID="AddEditServerRuleControl" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
</asp:Content>
