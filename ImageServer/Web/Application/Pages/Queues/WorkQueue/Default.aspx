<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Default"
    Title="Work Queue | ClearCanvas ImageServer" %>

<%@ Register Src="Edit/ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog" TagPrefix="localAsp" %>
<%@ Register Src="Edit/ResetWorkQueueDialog.ascx" TagName="ResetWorkQueueDialog"    TagPrefix="localAsp" %>        
<%@ Register Src="Edit/DeleteWorkQueueDialog.ascx" TagName="DeleteWorkQueueDialog"    TagPrefix="localAsp" %>        

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">

<script type="text/javascript">
    function Submit() {
        var isEnter = window.event == null ? e.keyCode == 13 : window.event.keyCode == 13;
        if(isEnter) {
            __doPostBack('<%=RefreshRateTextBox.ClientID %>','<%=RefreshRateTextBox.Text %>');     
        }
    }
</script>


<asp:Panel runat="server" style="position: relative">
<table width="100%" cellspacing="0" cellpadding="0">
<tr>
    <td>Work Queue</td>
    <td>
                                            <div style="position: absolute; right: 5px; top: 1px; font-size: 12px; width: 200px; text-align: right;">
                                                <span class="SearchTextBoxLabel" style="color: white">Refresh:</span>
                                                <asp:DropDownList ID="RefreshRateEnabled" runat="server" CssClass="SearchDropDownList" OnSelectedIndexChanged="RefreshRate_IndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Selected="True" Value="Y" Text="Yes"/>
                                                    <asp:ListItem Value="N" Text="No" />
                                                </asp:DropDownList> 
                                                <asp:TextBox ID="RefreshRateTextBox" runat="server" Width="30" Text="20" CssClass="SearchTextBoxLabel" onkeypress="javascript: Submit()" style="padding-left: 2px;" /><span class="SearchTextBoxLabel" style="color: white">s</span>
                                        </div> 
    </td>
</tr>
</table>

</asp:Panel>


</asp:Content>
  
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
