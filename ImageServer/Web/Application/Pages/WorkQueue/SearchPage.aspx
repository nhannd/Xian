<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="SearchPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.SearchPage"
    Title="Work Queue" %>

<%@ Register Src="Edit/ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="Edit/ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog"    TagPrefix="localAsp" %>        
<%@ Register Src="Edit/DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog"    TagPrefix="localAsp" %>        

<asp:Content ID="MainMenuContent" ContentPlaceHolderID="MainMenuPlaceHolder" runat="server">
    <asp:SiteMapDataSource ID="MainMenuSiteMapDataSource" runat="server" ShowStartingNode="False" />
    <asp:Menu runat="server" ID="MainMenu" SkinID="MainMenu" DataSourceID="MainMenuSiteMapDataSource" style="font-family: Sans-Serif"></asp:Menu>
</asp:Content>

<asp:Content ID="LocationName" ContentPlaceHolderID="LocationNamePlaceHolder" runat="server">Work Queue</asp:Content>
  
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
    <ccAsp:ConfirmationDialog runat="server" ID="ConfirmRescheduleDialog"/>
    <ccAsp:ConfirmationDialog runat="server" ID="InformationDialog" MessageType="INFORMATION" Title=""/>
    <localAsp:ScheduleWorkQueueDialog runat="server" ID="ScheduleWorkQueueDialog"/>
    <localAsp:ResetWorkQueueDialog ID="ResetWorkQueueDialog" runat="server" />
    <localAsp:DeleteWorkQueueDialog ID="DeleteWorkQueueDialog" runat="server" />
</asp:Content>
