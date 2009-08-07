<%@ Control Language="C#" AutoEventWireup="true" Codebehind="PatientSummaryPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.MoveSeries.PatientSummaryPanel" %>

<table width="100%" class="PatientSummaryTable">
        <tr>
            <td colspan="4">
                <ccUI:PersonNameLabel ID="personName" runat="server" PersonNameType="Dicom" CssClass="PatientName" /></td>
        </tr>
        <tr style="font-weight: bold; font-size: 14px;">
            <td>
                <asp:Label ID="Label2" runat="server" Text="ID: " CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientId" runat="server" Text="PatientId" CssClass="PatientInfo"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label3" runat="server" Text="DOB: " CssClass="PatientInfo"></asp:Label>
                <ccUI:DALabel ID="PatientDOB" runat="server" EmptyValueText="Unknown" CssClass="PatientInfo"></ccUI:DALabel>
            </td>
            <td>
                <asp:Label ID="Label4" runat="server" Text="Age: " CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientAge" runat="server" Text="PatientAge" CssClass="PatientInfo"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label5" runat="server" Text="Gender: " CssClass="PatientInfo"></asp:Label>
                <asp:Label ID="PatientSex" runat="server" Text="PatientSex" CssClass="PatientInfo"></asp:Label>
            </td>
        </tr>
    </table>
