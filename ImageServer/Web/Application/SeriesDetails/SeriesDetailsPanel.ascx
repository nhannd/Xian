<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SeriesDetailsPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.SeriesDetails.SeriesDetailsPanel" %>
<%@ Register Src="SeriesDetailsView.ascx" TagName="SeriesDetailsView" TagPrefix="uc1" %>
<asp:Panel ID="Panel1" runat="server">

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="StudyDetailsPanelContainer">
            <asp:Panel runat="server" ID="StudyDetailsViewPanel">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr valign="top">
                        <td width="50%">
                            <uc1:SeriesDetailsView id="SeriesDetailsView1" runat="server">
                            </uc1:SeriesDetailsView></td>
                        <td width="50%" valign="top" align="right">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
        </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>


</asp:Panel>
