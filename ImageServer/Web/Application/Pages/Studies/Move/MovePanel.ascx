<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>

<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel" %>
<%@ Import Namespace="Resources"%>

<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyGridView.ascx" TagName="StudyGridView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);

        function MultiSelect() {
            $("#<%=DeviceTypeFilter.ClientID %>").multiSelect({
                noneSelected: '',
                oneOrMoreSelected: '*',
                dropdownStyle: 'width: 160px'
            });
        }
    </script>
	
	<asp:Panel runat="server"  DefaultButton="SearchButton">
    
    <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="SeriesDetailsContent">
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr>
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Titles,Studies%>" /></td>
            <td align="right" valign="bottom">
                <asp:Panel CssClass="ToolbarButtons" style="padding-right: 4px;" runat="server">
                    <ccUI:ToolbarButton runat="server" SkinID="<%$Image:MoveButton%>" ID="MoveButton" onClick="MoveButton_Click" />
                    <ccUI:ToolbarButton runat="server" SkinID="<%$Image:DoneButton%>" ID="DoneButton" onClientClick="self.close();" />
                </asp:Panel>
            </td>
        </tr>
        <tr><td colspan="2">
            <asp:Panel ID="Panel1" runat="server" style="border: solid 1px #66aa65;">
                <localAsp:StudyGridView ID="StudyGridPanel" runat="server" />
            </asp:Panel>
        </td></tr>
    </table>
  </tr>

  <tr><td style="background-color: #66aa65"><asp:Image ID="Image2" runat="server" SkinID="Spacer" Height="4" /></td></tr>  

  <tr>
  <td class="ToolbarButtonPanel" style="width: 100%; color: #2A4C29; font-family: Sans-serif; font-weight: bold; font-size: 18px; padding-left: 5px; border-bottom: solid 1px #66aa65;">
  <%=Labels.DestinationDevices %></td>
  </tr>

  <tr>
  <td class="SeriesDetailsContent">
     <table cellpadding="2" cellspacing="0" class="ToolbarButtonPanel" width="100%" >
        <tr>
            <td align="left" nowrap="nowrap" valign="bottom" style="padding-top: 5px;"><asp:Label ID="Label1" runat="server" Text="<%$Resources: SearchFieldLabels, AETitle%>" CssClass="SearchTextBoxLabel" /><br /><asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" Tooltip="<%$ Resources: Tooltips, SearchByAETitle %>" /></td>
            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="<%$Resources: SearchFieldLabels, DeviceDescription %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="DescriptionFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$ Resources: Tooltips, SearchByAeDescription %>"></asp:TextBox></td>

            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="<%$Resources: SearchFieldLabels, IPAddress %>" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False"></asp:Label><br />
                                                <asp:TextBox ID="IPAddressFilter" runat="server" CssClass="SearchTextBox" ToolTip="<%$Resources: Tooltips, SearchByIpAddress %>"></asp:TextBox></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="<%$Resources: SearchFieldLabels, DHCP%>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:DropDownList ID="DHCPFilter" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="<%$Resources: SearchFieldLabels, DeviceType%>" CssClass="SearchTextBoxLabel"></asp:Label><br />
                                                <asp:ListBox ID="DeviceTypeFilter" runat="server" CssClass="SearchDropDownList" SelectionMode="Multiple">
                                                </asp:ListBox></td>
            <td align="right" nowrap="nowrap" style="padding-right: 6px; padding-bottom: 4px; vertical-align: bottom">
            <ccUI:ToolbarButton runat="server" ID="SearchButton" SkinID="<%$Image:SearchIcon%>" onClick="SearchButton_Click" /></td>
            <td width="100%">&nbsp;</td>
        </tr>

        <tr><td colspan="7">
            <asp:Panel runat="server" style="border: solid 1px #66aa65;">
            <table width="100%" style="background-color: #E1EFF8;" cellpadding="0" cellspacing="0">
                <tr><td ><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>
                <tr><td style="background-color: White;"><localAsp:DeviceGridView ID="DeviceGridPanel" runat="server" Height="500px" /></td></tr>
            </table>
            </asp:Panel>
        </td></tr>
    </table>
          </td>
  </tr>
  
  </table>
  
   </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<ccAsp:MessageBox ID="MoveConfirmation" runat="server" Title="<%$Resources: Titles, MoveStudy_Dialog_Confirmation %>"/>