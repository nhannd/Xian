<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc8" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<%@ Register Src="SearchToolBar.ascx" TagName="SearchToolBar" TagPrefix="uc2" %>
<%@ Register Src="SearchFilterPanel.ascx" TagName="SearchFilterPanel" TagPrefix="uc3" %>
<%@ Register Src="SearchGridView.ascx" TagName="SearchGridView" TagPrefix="uc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Style="padding-right: 2px; padding-left: 2px;
            padding-bottom: 2px; padding-top: 2px">
            <table width="100%">
                <tr class="toolBarPanel">
                    <td colspan="1" style="width: 250px">
                        <uc2:SearchToolBar ID="SearchToolBarControl" runat="server" />
                    </td>
                    <td align="right">
                        <uc3:SearchFilterPanel id="SearchFilterPanel" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" valign="top">
                        <uc1:SearchGridView ID="SearchGridViewControl" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <uc8:GridPager ID="GridPager1" runat="server"></uc8:GridPager>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
