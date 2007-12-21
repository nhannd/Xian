<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="SearchPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchPage"
    Title="ImageServer Search" %>

<%@ Register Src="../Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
                Work Queue
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent" CssClass="ContentWindow">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ajaxToolkit:TabContainer ID="WorkQueuePartitionTabContainer" runat="server" ActiveTabIndex="0"
                                CssClass="visoft__tab_xpie7">
                                <ajaxToolkit:TabPanel ID="WorkQueuePartitionTabPanel" runat="server" HeaderText="TabPanel1">
                                    <HeaderTemplate>
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
