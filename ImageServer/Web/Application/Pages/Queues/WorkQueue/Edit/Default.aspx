<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Pages/Common/MainContentSection.Master" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.Default" %>

<%@ Register Src="WorkQueueItemDetailsPanel.ascx" TagName="WorkQueueItemDetailsPanel"
    TagPrefix="localAsp" %>
<%@ Register Src="ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog" TagPrefix="localAsp" %>

<asp:Content runat="server" ID="MainMenuContent" contentplaceholderID="MainMenuPlaceHolder">
    <asp:Table ID="Table1" runat="server"><asp:TableRow><asp:TableCell HorizontalAlign="right" style="padding-top: 12px;"><asp:LinkButton ID="LinkButton1" runat="server" SkinId="CloseButton" OnClientClick="javascript: window.close(); return false;" /></asp:TableCell></asp:TableRow></asp:Table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContentSectionPlaceHolder">
            <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <localAsp:WorkQueueItemDetailsPanel ID="WorkQueueItemDetailsPanel" runat="server" />
                    <localAsp:ScheduleWorkQueueDialog ID="ScheduleWorkQueueDialog" runat="server" />
                    <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
                    <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
                    <ccAsp:MessageBox runat="server" ID="MessageBox" MessageType="INFORMATION" Title="" />
                    <center>
                        <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
                    </center>
                </ContentTemplate>
            </asp:UpdatePanel>
 </asp:Content>