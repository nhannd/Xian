<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.MovePanel" %>

<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="localAsp" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="localAsp" %>
<%@ Register Src="StudySummaryPanel.ascx" TagName="StudySummaryPanel" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
    <ccAsp:JQuery runat="server" ID="JQuery1" MultiSelect="true" Effects="true" />    
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
    
    <table cellpadding="0" cellspacing="0" width="100%">

  <tr>
    <td class="SeriesDetailsContent">
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td class="PatientInfo">
                    <localAsp:PatientSummaryPanel ID="PatientSummary" runat="server" />
                </td>
            </tr>
        </table>
    </td>
  </tr>
  
  <tr><td style="background-color: #3D98D1"><asp:Image ID="Image1" runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
<tr>
  <td class="SeriesDetailsContent">
      <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr><td class="MainContentSubTitle">Study Summary</td></tr>
        <tr><td>
        <localAsp:StudySummaryPanel ID="StudySummary" runat="server" />
        </td></tr>
    </table>
  </td>
  </tr>
  
  <tr><td style="background-color: #3D98D1"><asp:Image ID="Image2" runat="server" SkinID="Spacer" Height="4" /></td></tr>  
  
  <tr>
  <td class="SeriesDetailsContent">
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr>
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;">Series</td>
            <td align="right" valign="bottom">
                <asp:Panel CssClass="ToolbarButtons" style="padding-right: 4px;" runat="server">
                    <ccUI:ToolbarButton runat="server" SkinID="MoveButton" ID="MoveButton" onClick="MoveButton_Click" />
                    <ccUI:ToolbarButton runat="server" SkinID="DoneButton" ID="DoneButton" onClientClick="self.close();" />
                </asp:Panel>
            </td>
        </tr>
        <tr><td colspan="2">
            <asp:Panel ID="Panel1" runat="server" style="border: solid 1px #3d98d1;">
                <localAsp:SeriesGridView ID="SeriesGridPanel" runat="server" />
            </asp:Panel>
        </td></tr>
    </table>
  </tr>

  <tr><td style="background-color: #3D98D1"><asp:Image ID="Image3" runat="server" SkinID="Spacer" Height="4" /></td></tr>
  
  <tr>
  <td style="border-bottom: solid 1px #3d98d1;" class="ToolbarButtonPanel" style="width: 100%; color: #205F87; font-family: Sans-serif; font-weight: bold; font-size: 18px; padding-left: 5px;">Destination Devices</td>
  </tr>

  <tr>
  <td class="SeriesDetailsContent">
     <table cellpadding="2" cellspacing="0" class="ToolbarButtonPanel" width="100%" >
        <tr>
            <td align="left" nowrap="nowrap" valign="bottom" style="padding-top: 5px;"><asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="SearchTextBoxLabel" /><br /><asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" /></td>
            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Description" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Description"></asp:TextBox></td>
            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="IP Address" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by IP Address"></asp:TextBox></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="DHCP" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="Device Type" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:ListBox ID="DeviceTypeFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple">
                                                </asp:ListBox></td>
            <td align="right" nowrap="nowrap" style="padding-right: 6px; padding-bottom: 4px; vertical-align: bottom"><ccUI:ToolbarButton runat="server" ID="SearchButton" SkinID="SearchIcon" onClick="SearchButton_Click" /></td>
            <td width="100%">&nbsp;</td>
        </tr>

        <tr><td colspan="7">
            <asp:Panel runat="server" style="border: solid 1px #3d98d1;">
            <table width="100%" style="background-color: #E1EFF8;" cellpadding="0" cellspacing="0">
                <tr><td style="border-bottom: solid 1px #3d98d1;"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>
                <tr><td style="background-color: White;"><localAsp:DeviceGridView ID="DeviceGridPanel" runat="server" Height="500px" /></td></tr>
            </table>
            </asp:Panel>
        </td></tr>
    </table>
          </td>
  </tr>
  
  </table>
   
    </ContentTemplate>
</asp:UpdatePanel>

<ccAsp:MessageBox ID="MoveConfirmation" runat="server" Title="Move Series Confirmation"/>