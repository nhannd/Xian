<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="DevicePage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.Devices.DevicePage"
    Title="ImageServer Device Config" %>

<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc1" %>
<%@ Register Src="AddEditDeviceDialog.ascx" TagName="AddEditDeviceDialog" TagPrefix="uc2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="WindowTitleBar">
                Device Management
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent" CssClass="ContentWindow">
                    <asp:UpdatePanel ID="DevicePageUpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="visoft__tab_xpie7">
                                <ajaxToolkit:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
                                    <HeaderTemplate>
                                    </HeaderTemplate>
                                    <ContentTemplate>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                            <uc2:AddEditDeviceDialog ID="AddEditDeviceControl1" runat="server" />
                            <uc1:ConfirmDialog ID="ConfirmDialog1" runat="server" />
                            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
