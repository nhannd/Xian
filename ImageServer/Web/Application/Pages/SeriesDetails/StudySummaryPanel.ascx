<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudySummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.StudySummaryPanel" %>

<asp:Panel ID="StudySummaryPanelContainer" runat="server" Width="100%" 
    BorderStyle="solid" BorderWidth="0px" style="background-color: #ffffe8;">
    <asp:Panel ID="Panel1" runat="server" Style="padding-right: 5px; padding-left: 5px;
        padding-bottom: 5px; padding-top: 5px">
    <asp:Table ID="Table1" runat="server" Width="100%"  CellSpacing="5">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Label ID="Label1" runat="server" Text="Accession #: " Style="white-space: nowrap"></asp:Label>                                
                <asp:Label ID="AccessionNumber" runat="server" Text="" ></asp:Label>
            </asp:TableCell>
             <asp:TableCell>
                <asp:Label ID="Label2" runat="server" Text="Description: "  Style="white-space: nowrap"></asp:Label>
                <asp:Label ID="StudyDescription" runat="server" Text="Study Description" ></asp:Label>
            </asp:TableCell>
            
             <asp:TableCell>
                <asp:Label ID="Label3" runat="server" Text="Date: "  Style="white-space: nowrap"></asp:Label>
                <ccUI:DALabel ID="StudyDate" runat="server" InvalidValueText="[Invalid date: {0}]"></ccUI:DALabel>
            </asp:TableCell>
            <asp:TableCell>
                <asp:Label ID="Label4" runat="server" Text="Referring Physician: " Style="white-space: nowrap"></asp:Label>
                <ccUI:PersonNameLabel ID="ReferringPhysician" runat="server" PersonNameType="Dicom"></ccUI:PersonNameLabel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </asp:Panel>
</asp:Panel>
