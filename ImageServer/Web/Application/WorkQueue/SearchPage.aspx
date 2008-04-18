<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="SearchPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchPage"
    Title="ImageServer Search" %>

<%@ Register Src="~/Common/ServerPartitionTabs.ascx" TagName="ServerPartitionTabs" TagPrefix="ccPartitionTabs" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="~/WorkQueue/Edit/ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="~/WorkQueue/Edit/ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog"    TagPrefix="clearcanvas" %>        
<%@ Register Src="~/WorkQueue/Edit/DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog"    TagPrefix="clearcanvas" %>        
  
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent">
                    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <ccPartitionTabs:ServerPartitionTabs ID="ServerPartitionTabs" runat="server" />
                            <asp:Label ID="Label1" runat="server" Style="left: 70px; position: relative;" Text="Label"
                                Visible="False" Width="305px"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <clearcanvas:ConfirmationDialog runat="server" ID="ConfirmRescheduleDialog"/>
    <clearcanvas:ConfirmationDialog runat="server" ID="InformationDialog" MessageType="INFORMATION" Title=""/>
    <clearcanvas:ScheduleWorkQueueDialog runat="server" ID="ScheduleWorkQueueDialog"/>
    <clearcanvas:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
    <clearcanvas:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
</asp:Content>
