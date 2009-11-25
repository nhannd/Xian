<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DevicePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <script>
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);

        function MultiSelect() {
            $("#<%=DeviceTypeFilter.ClientID %>").multiSelect({
                noneSelected: '',
                oneOrMoreSelected: '*',
                style: 'width: 160px'
            });
        }
        </script>

        <asp:Panel ID="Panel1" runat="server">
            <asp:Table ID="Table" runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False"></asp:Label><br />
                                        <asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by AE Title"></asp:TextBox>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label6" runat="server" Text="Description" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False"></asp:Label><br />
                                        <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Description"></asp:TextBox>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="Label2" runat="server" Text="IP Address" CssClass="SearchTextBoxLabel"
                                            EnableViewState="False"></asp:Label><br />
                                        <asp:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by IP Address"></asp:TextBox>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label3" runat="server" Text="Status" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                        <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label4" runat="server" Text="DHCP" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                        <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList">
                                        </asp:DropDownList>
                                    </td>
                                    <td align="left" valign="bottom">
                                        <asp:Label ID="Label5" runat="server" Text="Device Type" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                        <asp:ListBox ID="DeviceTypeFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple">
                                        </asp:ListBox>
                                    </td>
                                    <td align="right" valign="bottom">
                                        <asp:Panel ID="Panel3" runat="server" CssClass="SearchButtonPanel">
                                            <ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click" /></asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell>
                        <table width="100%" cellpadding="3" cellspacing="0" class="ToolbarButtonPanel">
                            <tr>
                                <td>
                                    <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                                <ccUI:ToolbarButton ID="AddDeviceButton" runat="server" SkinID="AddButton" OnClick="AddDeviceButton_Click" />
                                                <ccUI:ToolbarButton ID="EditDeviceButton" runat="server" SkinID="EditButton" OnClick="EditDeviceButton_Click" />
                                                <ccUI:ToolbarButton ID="DeleteDeviceButton" runat="server" SkinID="DeleteButton"
                                                    OnClick="DeleteDeviceButton_Click" />
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Panel ID="Panel2" runat="server" Style="border: solid 1px #3d98d1;">
                                        <table width="100%" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td style="border-bottom: solid 1px #3d98d1">
                                                    <ccAsp:GridPager ID="GridPagerTop" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="background-color: white;">
                                                    <localAsp:DeviceGridView ID="DeviceGridViewControl1" Height="500px" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        </div> <b class="roundedCorners"><b class="roundedCorners5"></b><b class="roundedCorners4">
        </b><b class="roundedCorners3"></b><b class="roundedCorners2"><b></b></b><b class="roundedCorners1">
            <b></b></b></b></div>
    </ContentTemplate>
</asp:UpdatePanel>
