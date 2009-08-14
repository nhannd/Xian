<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.StudySummaryPanel" %>
<table width="100%" class="StudySummary" cellpadding="0" cellspacing="0">
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label1" runat="server" Text="Accession #: " Style="white-space: nowrap"></asp:Label></td>
                    <td>
                        <asp:Label ID="AccessionNumber" runat="server" Text=""></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding=02" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label2" runat="server" Text="Description: " Style="white-space: nowrap"></asp:Label></td>
                    <td style="border-bottom: solid 2px #eeeeee">
                        <asp:Label ID="StudyDescription" runat="server" Text="Study Description"></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td class="StudySummaryRow">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label3" runat="server" Text="Date: " Style="white-space: nowrap"></asp:Label></td>
                    <td style="border-bottom: solid 2px #eeeeee">
                        <ccUI:DALabel ID="StudyDate" runat="server" InvalidValueText="[Invalid date: {0}]"></ccUI:DALabel></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="StudySummaryHeader">
                        <asp:Label ID="Label4" runat="server" Text="Referring Physician: " Style="white-space: nowrap"></asp:Label></td>
                    <td>
                        <ccUI:PersonNameLabel ID="ReferringPhysician" runat="server" PersonNameType="Dicom"></ccUI:PersonNameLabel></td>
            </table>
        </td>
    </tr>
</table>
