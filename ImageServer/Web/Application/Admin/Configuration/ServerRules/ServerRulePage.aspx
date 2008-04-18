<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    Codebehind="ServerRulePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules.ServerRulePage"
    Title="ImageServer Rules Engine" ValidateRequest="false" %>

<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs" TagPrefix="uc1" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="AddEditServerRuleDialog.ascx" TagName="AddEditServerRuleDialog"TagPrefix="uc3" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Rules Engine Management
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="ServerRulePageUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc1:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                <clearcanvas:ConfirmationDialog ID="ConfirmDialog" runat="server" />
                <uc3:AddEditServerRuleDialog ID="AddEditServerRuleControl" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
