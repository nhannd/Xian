<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Codebehind="ServerRulePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRulePage"
    Title="ImageServer Rules Engine" ValidateRequest="false" %>

<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs"
    TagPrefix="ccPartitionTabs" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="ccConfirm" %>
<%@ Register Src="AddEditServerRuleDialog.ascx" TagName="AddEditServerRuleDialog"
    TagPrefix="ccAddEdit" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Rules Engine Management
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="ServerRulePageUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccPartitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                <ccConfirm:ConfirmDialog ID="ConfirmDialog" runat="server" />
                <ccAddEdit:AddEditServerRuleDialog ID="AddEditServerRuleControl" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
