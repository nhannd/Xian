<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Default"
    Title="Work Queue | ClearCanvas ImageServer" %>

<%@ Register Src="Edit/ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="Edit/ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog"    TagPrefix="localAsp" %>        
<%@ Register Src="Edit/DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog"    TagPrefix="localAsp" %>        

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Work Queue</asp:Content>
  
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent">
                    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ccAsp:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <ccAsp:MessageBox runat="server" ID="ConfirmRescheduleDialog"/>
    <ccAsp:MessageBox runat="server" ID="InformationDialog" MessageType="INFORMATION" Title=""/>
    <localAsp:ScheduleWorkQueueDialog runat="server" ID="ScheduleWorkQueueDialog"/>
    <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
    <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
</asp:Content>
