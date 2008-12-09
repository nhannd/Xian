<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Studies.Default"
    Title="Studies | ClearCanvas ImageServer" %>

<%@ Register Src="StudyDetails/Controls/DeleteStudyConfirmDialog.ascx" TagName="DeleteStudyConfirmDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder">Studies</asp:Content>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:Panel runat="server" ID="PageContent">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" Visible="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>    

    <localAsp:DeleteStudyConfirmDialog ID="DeleteStudyConfirmDialog" runat="server"/>

</asp:Content>
