<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchPanel" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc5" %>
<%@ Register Src="SearchFilterPanel.ascx" TagName="SearchFilterPanel" TagPrefix="uc3" %>
<%@ Register Src="SearchAccordian.ascx" TagName="SearchAccordian" TagPrefix="accordian" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Style="padding-right: 2px; padding-left: 2px;
            padding-bottom: 2px; padding-top: 2px">
            <table width="100%">
                <tr class="toolBarPanel">
                    <td align="right">
                        <uc3:SearchFilterPanel ID="SearchFilterPanel" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <accordian:SearchAccordian ID="SearchAccordianControl" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
