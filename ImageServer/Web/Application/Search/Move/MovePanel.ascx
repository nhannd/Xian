<%@ Control Language="C#" AutoEventWireup="true" Codebehind="MovePanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Search.Move.MovePanel" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc1" %>
<%@ Register Src="DeviceGridView.ascx" TagName="DeviceGridView" TagPrefix="uc2" %>
<%@ Register Src="StudyGridView.ascx" TagName="StudyGridView" TagPrefix="uc3" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/SectionPanel.ascx" TagName="SectionPanel" TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/TimedDialog.ascx" TagName="TimedDialog" TagPrefix="clearcanvas" %>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <clearcanvas:SectionPanel ID="StudySectionPanel" runat="server" HeadingText="Studies to Move"
            HeadingCSS="CSSStudyHeading" Width="100%" CssClass="CSSSection">
            <SectionContentTemplate>
            <br />
                <asp:Panel ID="Panel10" runat="server" CssClass="CSSGridViewPanelBorder">
                    <uc3:StudyGridView ID="StudyGridPanel" runat="server" Height="500px" />
                </asp:Panel>
            </SectionContentTemplate>
        </clearcanvas:SectionPanel>
        <br />
        <clearcanvas:SectionPanel ID="DeviceSectionPanel" runat="server" HeadingText="Move Destination Devices"
            HeadingCSS="CSSStudyHeading" Width="100%" CssClass="CSSSection">
            <SectionContentTemplate>
                <asp:Panel ID="Panel1" runat="server" Height="100%">
                    <asp:Table ID="Table" runat="server" Width="100%" Height="100%" CellPadding="0" BorderWidth="0px">
                        <asp:TableHeaderRow VerticalAlign="top">
                            <asp:TableHeaderCell HorizontalAlign="left" VerticalAlign="Bottom" Wrap="false" Width="100%">
                                <asp:Panel ID="Panel9" runat="server" CssClass="CSSToolbarPanelContainer">
                                    <asp:Panel ID="Panel11" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                        <asp:Panel ID="Panel12" runat="server" CssClass="CSSToolbarContent">
                                            <clearcanvas:ToolbarButton ID="SendToolbarButton" runat="server" EnabledImageURL="~/images/icons/SendEnabled.png"
                                                DisabledImageURL="~/images/icons/SendDisabled.png" OnClick="SendButton_Click"
                                                AlternateText="Send Studies to device" />
                                        </asp:Panel>
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell HorizontalAlign="right" Width="100%" Wrap="false">
                                <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                                    <asp:Panel ID="Panel3" runat="server" CssClass="CSSFilterPanelBorder">
                                        <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label1" runat="server" Text="AE Title" CssClass="CSSTextBoxLabel"></asp:Label><br />
                                                        <asp:TextBox ID="AETitleFilter" runat="server" CssClass="CSSFilterTextBox"></asp:TextBox>
                                                    </td>
                                                    <td align="right" valign="bottom">
                                                        <asp:Panel ID="FilterButtonContainer" runat="server" CssClass="FilterButtonContainer">
                                                            <clearcanvas:ToolbarButton ID="FilterToolbarButton" runat="server" EnabledImageURL="~/images/icons/QueryEnabled.png"
                                                                DisabledImageURL="~/images/icons/QueryDisabled.png" OnClick="FilterButton_Click"
                                                                ToolTip="Filter/Refresh" />
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                        <asp:TableRow VerticalAlign="Top">
                            <asp:TableCell ColumnSpan="2">
                                <asp:Panel ID="Panel7" runat="server" CssClass="CSSGridViewPanelContainer">
                                    <asp:Panel ID="Panel8" runat="server" CssClass="CSSGridViewPanelBorder">
                                        <uc2:DeviceGridView ID="DeviceGridPanel" runat="server" Height="500px" />
                                    </asp:Panel>
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableFooterRow VerticalAlign="bottom">
                            <asp:TableCell ColumnSpan="2">
                                <uc1:GridPager ID="GridPager" runat="server" />
                            </asp:TableCell>
                        </asp:TableFooterRow>
                    </asp:Table>
                </asp:Panel>
            </SectionContentTemplate>
        </clearcanvas:SectionPanel>
                <clearcanvas:TimedDialog ID="TimedDialog" runat="server" Timeout="5000" ShowOkButton="false" Title="Move Complete" />
    </ContentTemplate>
</asp:UpdatePanel>
