<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchResultAccordian.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchResultAccordian" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Panel ID="Panel1" runat="server" CssClass="CSSAccordianResultPanelContainer">
    <asp:Panel ID="Panel4" runat="server" CssClass="CSSAccordianResultPanelBorder">
        <asp:Panel ID="Panel5" runat="server" CssClass="CSSAccordianResultPanelContent">
            <asp:Table ID="Table1" runat="server" Width="100%" CellPadding="0" CellSpacing="0"
                BorderWidth="0px">
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Panel8" runat="server" CssClass="CSSAccordianHeaderPanelContainer">
                            <asp:Panel ID="Panel9" runat="server" CssClass="CSSAccordianHeaderPanelBorder">
                                <asp:Panel ID="Panel10" runat="server" CssClass="CSSAccordianHeaderPanelContent">
                                    <asp:Label ID="PatientID" runat="server" CssClass="CSSAccordianHeaderText" Text="Patient ID"
                                        EnableViewState="False" Width="15%" />
                                    <asp:Label ID="PatientsName" runat="server" CssClass="CSSAccordianHeaderText" Text="Patient Name"
                                        EnableViewState="False" Width="30%" />
                                    <asp:Label ID="WorkQueueType" runat="server" CssClass="CSSAccordianHeaderText" Text="Type"
                                        EnableViewState="False" Width="15%" />
                                    <asp:Label ID="WorkQueueStatus" runat="server" CssClass="CSSAccordianHeaderText"
                                        Text="Status" EnableViewState="False" Width="15%" />
                                    <asp:Label ID="ScheduledTime" runat="server" CssClass="CSSAccordianHeaderText" Text="Schedule"
                                        EnableViewState="False" />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSAccordianResultListContainer"
                            ScrollBars="Vertical">
                            <ajaxToolkit:Accordion ID="MainAccordian" runat="Server" HeaderSelectedCssClass="CSSAccordianStudyHeaderSelected"
                                HeaderCssClass="CSSAccordianStudyHeader" AutoSize="None" FadeTransitions="true"
                                TransitionDuration="250" FramesPerSecond="40" RequireOpenedPane="false" SuppressHeaderPostbacks="true">
                                <Panes>
                                </Panes>
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ContentTemplate>
                                </ContentTemplate>
                            </ajaxToolkit:Accordion>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableFooterRow Width="100%">
                    <asp:TableCell>
                        <asp:Panel ID="Panel3" runat="server" CssClass="CSSGridPagerPanelContainer">
                            <asp:Panel ID="Panel6" runat="server" CssClass="CSSGridPagerPanelBorder">
                                <asp:Panel ID="Panel7" runat="server" CssClass="CSSGridPagerPanelContent">
                                    <table width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center" valign="middle">
                                                <asp:Label ID="PagerStudyCountLabel" runat="server" Text="Label"></asp:Label></td>
                                            <td align="center" valign="middle">
                                                <asp:Label ID="PagerPagingLabel" runat="server" Text="Label"></asp:Label></td>
                                            <td align="right" valign="middle">
                                                <asp:ImageButton ID="PrevPageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                                                    OnCommand="PageButtonClick" />
                                                <asp:ImageButton ID="NextPageButton" runat="server" CommandArgument="Next" CommandName="Page"
                                                    OnCommand="PageButtonClick" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableFooterRow>
            </asp:Table>
            <asp:HiddenField ID="SelectedWorkQueueGUID" runat="server" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
