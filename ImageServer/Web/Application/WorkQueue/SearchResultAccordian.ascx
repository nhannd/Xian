<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchResultAccordian.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchResultAccordian" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Panel runat="server">
    <asp:Table runat="server" Width="100%">
        <asp:TableHeaderRow>
            <asp:TableHeaderCell>
            <asp:Panel ID="HeaderPanel" runat="server" Width="100%" BorderWidth="0px" CssClass="GridHeader">
                <asp:Label ID="PatientID" runat="server" Text="Patient ID" EnableViewState="False"
                    CssClass="GridHeaderText" Width="100px" />
                <asp:Label ID="PatientsName" runat="server" Text="Patient's Name" EnableViewState="False"
                    CssClass="GridHeaderText" Width="230px" />
                <asp:Label ID="WorkQueueType" runat="server" Text="Type" EnableViewState="False"
                    CssClass="GridHeaderText" Width="100px" />
                <asp:Label ID="WorkQueueStatus" runat="server" Text="Status" EnableViewState="False"
                    CssClass="GridHeaderText" Width="80px" />
                <asp:Label ID="ScheduledTime" runat="server" Text="Schedule" EnableViewState="False"
                    CssClass="GridHeaderText" Width="150px" />
            </asp:Panel>
            </asp:TableHeaderCell>
            
        </asp:TableHeaderRow>
        <asp:TableRow>
            <asp:TableCell>
            <asp:Panel ID="Panel2" runat="server" CssClass="GridPanel" ScrollBars="Vertical">
                <ajaxToolkit:Accordion ID="MainAccordian" runat="Server" HeaderSelectedCssClass="accordionHeaderSelected"
                    AutoSize="None" FadeTransitions="true" TransitionDuration="250" FramesPerSecond="40"
                    RequireOpenedPane="false" SuppressHeaderPostbacks="true" HeaderCssClass="accordionHeader">
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
        <asp:TableFooterRow Width="100%" BackColor="Gainsboro" BorderColor="Silver" BorderStyle="Solid"
            BorderWidth="1px">
            <asp:TableCell>
            <asp:Panel ID="Panel1" runat="server" CssClass="GridPagerStyle">
                <table width="100%">
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

</asp:TableCell>
                    </asp:TableFooterRow>
    </asp:Table>
</asp:Panel>
