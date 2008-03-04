<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.SeriesDetails.StudySummaryPanel" %>
<asp:Panel ID="StudySummaryPanelContainer" runat="server" Width="100%" 
    BorderStyle="solid" BorderWidth="0px" style="background-color: #ffffe8;">
    <asp:Panel ID="Panel1" runat="server" Style="padding-right: 10px; padding-left: 10px;
        padding-bottom: 10px; padding-top: 10px">
    <asp:Table ID="Table1" runat="server" Width="100%" style="font-size:medium;">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label1" runat="server" Text="Accession #: " Style="white-space: nowrap"></asp:Label>                                
                <asp:Label ID="AccessionNumber" runat="server" Text="" ></asp:Label>
            </asp:TableCell>
             <asp:TableCell>
                <asp:Label ID="Label2" runat="server" Text="Study Description: "  Style="white-space: nowrap"></asp:Label>
                <asp:Label ID="StudyDescription" runat="server" Text="Study Description" ></asp:Label>
            </asp:TableCell>
            
             <asp:TableCell>
                <asp:Label ID="Label3" runat="server" Text="Study Date: "  Style="white-space: nowrap"></asp:Label>
                <asp:Label ID="StudyDate" runat="server" Text="Study Date" ></asp:Label>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="Label4" runat="server" Text="Referring Physician: " Style="white-space: nowrap"></asp:Label>
                <asp:Label ID="ReferringPhysician" runat="server" Text="ReferringPhysician" ></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Panel>
</asp:Panel>
