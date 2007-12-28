<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchAccordian.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchAccordian" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Panel runat="server">
    <asp:Table runat="server" Width="100%" BorderColor="Silver" BorderWidth="1px" BorderStyle="Solid"
        CellSpacing="1">
        <asp:TableRow BorderWidth="0px">
            <asp:TableCell BorderWidth="0px">
                <asp:Panel ID="HeaderPanel" runat="server" Width="100%" BorderWidth="1px" BorderColor="Silver"
                    CssClass="GridHeader">
                    <asp:Label ID="PatientName" runat="server" Text="Patient Name" EnableViewState="False"
                        CssClass="GridHeaderText" Width="24%" />
                    <asp:Label ID="PatientId" runat="server" Text="Patient Id" EnableViewState="False"
                        CssClass="GridHeaderText" Width="12%" />
                    <asp:Label ID="StudyDate" runat="server" Text="Study Date" EnableViewState="False"
                        CssClass="GridHeaderText" Width="12%" />
                    <asp:Label ID="AccessionNumber" runat="server" Text="Accession Number" EnableViewState="False"
                        CssClass="GridHeaderText" Width="19%" />
                    <asp:Label ID="StudyDescription" runat="server" Text="Description" EnableViewState="False"
                        CssClass="GridHeaderText" Width="23%" />
                </asp:Panel>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Panel runat="server" CssClass="GridPanel" ScrollBars="Vertical">
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
