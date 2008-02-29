<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PatientSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.PatientSummaryPanel" %>
<asp:Panel ID="PatientSummaryPanelContainer" runat="server" Width="100%" 
    BorderStyle="Outset" BorderWidth="1px" style="background-color: #0099ff">
    <asp:Panel ID="Panel1" runat="server" Style="padding-right: 10px; padding-left: 10px;
        padding-bottom: 10px; padding-top: 10px">
    <asp:Table ID="Table1" runat="server" Width="100%" style="font-size: larger">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="PatientName" runat="server" Text="PatientName" ForeColor="white"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="PatientId" runat="server" Text="PatientId" ForeColor="white"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="PatientBirthDate" runat="server" Text="PatientBirthDate" ForeColor="white"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="PatientAge" runat="server" Text="PatientAge" ForeColor="white"></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="PatientSex" runat="server" Text="PatientSex" ForeColor="white"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Panel>
</asp:Panel>
