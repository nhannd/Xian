<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Common/Pages/MainContentSection.Master" Title="ClearCanvas ImageServer" Codebehind="ViewEdit.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.ViewEdit" %>

<%@ Register Src="WorkQueueItemDetailsPanel.ascx" TagName="WorkQueueItemDetailsPanel"
    TagPrefix="localAsp" %>
<%@ Register Src="ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <table width="100%"><tr><td align="right" style="padding-top: 17px;"><asp:LinkButton ID="LinkButton1" runat="server" Text="Close" Font-Size="18px" Font-Bold="true" ForeColor="white" Font-Underline="false" OnClientClick="javascript: window.close(); return false;" /></td></tr></table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentSection">
            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <localAsp:WorkQueueItemDetailsPanel ID="WorkQueueItemDetailsPanel" runat="server" />
                    <localAsp:ScheduleWorkQueueDialog ID="ScheduleWorkQueueDialog" runat="server" />
                    <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
                    <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
                    <ccAsp:ConfirmationDialog runat="server" ID="InformationDialog" MessageType="INFORMATION" Title="" />
                    <center>
                        <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
                    </center>
                </ContentTemplate>
            </asp:UpdatePanel>
 </asp:Content>