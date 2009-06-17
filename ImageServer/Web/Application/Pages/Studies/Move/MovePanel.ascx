<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Move.MovePanel" %>

<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyGridView.ascx" TagName="StudyGridView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
    <script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);
    </script>
    
    <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="SeriesDetailsContent">
    <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr>
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;">Studies</td>
            <td align="right" valign="bottom">
                <asp:Panel CssClass="ToolbarButtons" style="padding-right: 4px;" runat="server">
                    <ccUI:ToolbarButton runat="server" SkinID="MoveButton" ID="MoveButton" onClick="MoveButton_Click" />
                    <ccUI:ToolbarButton runat="server" SkinID="CancelButton" ID="CancelButton" onClientClick="self.close();" />
                </asp:Panel>
            </td>
        </tr>
        <tr><td colspan="2">
            <asp:Panel ID="Panel1" runat="server" style="border: solid 1px #3d98d1;">
                <localAsp:StudyGridView ID="StudyGridPanel" runat="server" />
            </asp:Panel>
        </td></tr>
    </table>
  </tr>

  <tr><td style="background-color: #8FC3E4; border-top: solid 1px #3d98d1; border-bottom: solid 1px #3d98d1;"><asp:image runat="server" SkinID="Spacer" height="5px" /></td></tr>

  <tr>
  <td class="SeriesDetailsContent">
     <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
        <tr >
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px; width: 100%">Destination Devices</td>
            <td align="right" nowrap="nowrap"><asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="SearchLabelDark" /><asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" /></td>
            <td align="right" nowrap="nowrap" style="padding-right: 6px;"><ccUI:ToolbarButton runat="server" ID="SearchButton" SkinID="SearchButton" onClick="SearchButton_Click" /></td>
        </tr>

        <tr><td colspan="3">
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

<ccAsp:MessageBox ID="MoveConfirmation" runat="server" Title="Move Confirmation"/>