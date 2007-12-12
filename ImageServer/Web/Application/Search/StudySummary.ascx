<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummary.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Search.StudySummary" %>
<asp:Panel ID="DetailsPanel" runat="server" BorderWidth="0px">
    <table cellpadding="2" cellspacing="1" border="0" class="GridRowStyle">
        <tr>
            <td align="left" valign="bottom" style="word-wrap: break-word; width: 170px">
                <asp:Label ID="PatientName" runat="server" Text="Patient Name" EnableViewState="False" />
            </td>
            <td align="left" valign="bottom" style="word-wrap: break-word; width: 100px">
                <asp:Label ID="PatientId" runat="server" Text="Patient ID" EnableViewState="False" />
            </td>
            <td align="left" valign="bottom" style="word-wrap: break-word; width: 90px">
                <asp:Label ID="StudyDate" runat="server" Text="Study Date" EnableViewState="False" />
            </td>
            <td align="left" valign="bottom" style="word-wrap: break-word; width: 150px">
                <asp:Label ID="AccessionNumber" runat="server" Text="Accession Number" EnableViewState="False" />
            </td>
            <td align="left" valign="bottom" style="word-wrap: break-word; width: 192px">
                <asp:Label ID="StudyDescription" runat="server" Text="Description" EnableViewState="False" />
            </td>
        </tr>
    </table>
</asp:Panel>
