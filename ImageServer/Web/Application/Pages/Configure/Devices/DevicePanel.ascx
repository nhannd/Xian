<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DevicePanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.Devices.DevicePanel" %>

<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        
        <div>
        <b class="roundedCorners"><b class="roundedCorners1"><b></b></b><b class="roundedCorners2">
            <b></b></b><b class="roundedCorners3"></b><b class="roundedCorners4"></b><b class="roundedCorners5">
            </b></b>
        <div class="roundedCornersfg">
        
        <asp:Panel ID="Panel1" runat="server">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0"
                BorderWidth="0px">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="right">
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchToolbarButton">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by AE Title"></asp:TextBox></td>
                                            <td align="left">
                                                <asp:Label ID="Label2" runat="server" Text="IP Address" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by IP Address"></asp:TextBox></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Status" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="StatusFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="DHCP" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="right" valign="bottom">
                                                <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="SearchButtonContainer">                                                        
                                                    <asp:ImageButton ID="SearchToolbarButton" runat="server" SkinID="SearchButton" OnClick="SearchButton_Click" Tooltip="Search for devices" />
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>                    
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow Height="100%">
                    <asp:TableCell>
                                            <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons">
                                        <ccUI:ToolbarButton ID="AddDeviceButton" runat="server" SkinID="AddButton" />
                                        <ccUI:ToolbarButton ID="EditDeviceButton" runat="server" SkinID="EditButton" />
                                        <ccUI:ToolbarButton ID="DeleteDeviceButton" runat="server" SkinID="DeleteButton" />
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:DeviceGridView ID="DeviceGridViewControl1" Height="500px" runat="server" /></localAsp:StudyListGridView></td></tr>
                                <tr><td style="border-top: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerBottom" runat="server" /></td></tr>                    
                            </table>                        
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>

        </div>
        <b class="roundedCorners"><b class="roundedCorners5"></b><b class="roundedCorners4">
        </b><b class="roundedCorners3"></b><b class="roundedCorners2"><b></b></b><b class="roundedCorners1">
            <b></b></b></b>
    </div>
        
        <ccAsp:ConfirmationDialog ID="ConfirmDialog1" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
