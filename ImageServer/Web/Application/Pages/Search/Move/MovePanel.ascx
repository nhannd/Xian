<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Search.Move.MovePanel" %>

<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="localAsp" %>
<%@ Register Src="StudyGridView.ascx" TagName="StudyGridView" TagPrefix="localAsp" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
    <table cellpadding="0" cellspacing="0" width="100%">
  
  <tr>
  <td class="MainContentTitle">Move Studies</td>  
  </tr>
  
  <tr>
  <td class="SeriesDetailsContent">
    <table width="100%" cellpadding="2" cellspacing="0" style="background-color: #B8D9EE;">
        <tr>
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;">Studies</td>
            <td class="ButtonPanel"><asp:Button runat="server" ID="MoveButton" Text="Move" CssClass="ButtonStyle" /><asp:Button runat="server" ID="CancelButton" Text="Cancel" CssClass="ButtonStyle" OnClientClick="history.back();" /></td>
        </tr>
        <tr><td colspan="2">
            <asp:Panel ID="Panel1" runat="server" style="border: solid 1px #3d98d1;">
                <localAsp:StudyGridView ID="StudyGridPanel" runat="server" />
            </asp:Panel>
        </td></tr>
    </table>
  </tr>

  <tr><td><img src="~/images/blank.gif" height="3px" /></td></tr>

  <tr>
  <td class="SeriesDetailsContent">
     <table width="100%" cellpadding="2" cellspacing="0" style="background-color: #B8D9EE">
        <tr>
            <td class="MainContentSubTitle" style="vertical-align: bottom; padding-top: 5px;">Destination Devices</td>
            <td align="right" style="vertical-align: bottom"><asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="SearchLabel" /><asp:TextBox ID="AETitleFilter" runat="server" CssClass="SearchTextBox" /> <asp:Button runat="server" ID="SearchButton" Text="Search" CssClass="ButtonStyle" />
        </tr>

        <tr><td colspan="2">
            <asp:Panel runat="server" style="border: solid 1px #3d98d1; ">
            <table width="100%" style="background-color: #E1EFF8;" cellpadding="0">
                <tr><td style="border-bottom: solid 1px #3d98d1;"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>
                <tr><td style="background-color: White;"><localAsp:DeviceGridView ID="DeviceGridPanel" runat="server" Height="500px" /></td></tr>
                <tr><td style="border-top: solid 1px #3d98d1;"><ccAsp:GridPager ID="GridPagerBottom" runat="server" /></td></tr>
            </table>
            </asp:Panel>
        </td></tr>
    </table>
          </td>
  </tr>
  
  </table>
    
            <ccAsp:TimedDialog ID="TimedDialog" runat="server" Timeout="5000" ShowOkButton="false" Title="Move Complete" />
    </ContentTemplate>
</asp:UpdatePanel>
