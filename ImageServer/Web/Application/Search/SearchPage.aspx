<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="SearchPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchPage"
    Title="ImageServer Search" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="ccConfirm" %>
<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs"
    TagPrefix="ccPartitionTabs" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    Search
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccPartitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                <ccConfirm:ConfirmDialog ID="ConfirmDialog1" runat="server" />
                <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                    Visible="False" Width="305px"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
