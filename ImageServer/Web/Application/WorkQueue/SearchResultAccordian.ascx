<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchResultAccordian.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchResultAccordian" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Table Width="100%">
    <asp:TableHeaderRow>
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
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:Panel runat="server" BorderColor="Silver" BorderWidth="1px" Height="400px" ScrollBars="Vertical"
            Width="100%">
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
    </asp:TableRow>
    <asp:TableFooterRow Width="100%" BackColor="Gainsboro" BorderColor="Silver" BorderStyle="Solid"
        BorderWidth="1px">
        <asp:Panel ID="Panel1" runat="server" Height="24px" Width="100%" BackColor="Gainsboro"
            BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px">
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
    </asp:TableFooterRow>
</asp:Table>
