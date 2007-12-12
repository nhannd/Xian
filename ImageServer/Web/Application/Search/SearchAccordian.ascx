<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchAccordian.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.SearchAccordian" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Table Width="100%">
    <asp:TableHeaderRow>
        <asp:Panel ID="HeaderPanel" runat="server" Width="100%" BorderWidth="0px">
            <table cellpadding="2" cellspacing="1" class="GridHeader" border="0" width="100%">
                <tr>
                    <td align="left" valign="bottom" style="width: 170px">
                        <asp:Label ID="PatientName" runat="server" Text="Patient Name" EnableViewState="False" />
                    </td>
                    <td align="left" valign="bottom" style="width: 100px">
                        <asp:Label ID="PatientId" runat="server" Text="Patient ID" EnableViewState="False" />
                    </td>
                    <td align="left" valign="bottom" style="width: 90px">
                        <asp:Label ID="StudyDate" runat="server" Text="Study Date" EnableViewState="False" />
                    </td>
                    <td align="left" valign="bottom" style="width: 150px">
                        <asp:Label ID="AccessionNumber" runat="server" Text="Accession Number" EnableViewState="False" />
                    </td>
                    <td align="left" valign="bottom">
                        <asp:Label ID="StudyDescription" runat="server" Text="Description" EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:Panel runat="server" BorderColor="Silver" BorderWidth="1px" Height="400px" ScrollBars="Vertical"
            Width="100%">
            <ajaxToolkit:Accordion ID="MainAccordian" runat="Server" HeaderSelectedCssClass="accordionHeaderSelected"
                AutoSize="None" FadeTransitions="true" TransitionDuration="250" FramesPerSecond="40"
                RequireOpenedPane="false" SuppressHeaderPostbacks="true">
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
                            OnCommand="PageButtonClick" /></td>
                </tr>
            </table>
        </asp:Panel>
    </asp:TableFooterRow>
</asp:Table>
