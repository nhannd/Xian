<%@ Import namespace="ClearCanvas.Dicom"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DeletedStudyArchiveInfoPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudyArchiveInfoPanel" %>

<asp:Panel ID="Panel3" runat="server">
    <asp:Panel ID="NoArchiveMessagePanel" runat="server" CssClass="EmptySearchResultsMessage">
        <asp:Label ID="Label1" runat="server"  Text = "This study was not archived." />
    </asp:Panel>
    
    <asp:PlaceHolder runat="server" ID="ArchiveViewPlaceHolder">
    </asp:PlaceHolder>
    
</asp:Panel>