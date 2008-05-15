<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true" Codebehind="SeriesDetailsPage.aspx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsPage" %>

<%@ Register Src="SeriesDetailsPanel.ascx" TagName="SeriesDetailsPanel" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
            <asp:UpdatePanel runat="server" ID="updatepanel1" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="Panel1" runat="server" Width="100%">
                        <uc1:SeriesDetailsPanel id="SeriesDetailsPanel1" runat="server">
                    </uc1:SeriesDetailsPanel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>