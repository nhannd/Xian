<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.SearchPanel" %>
<%@ Register Src="WorkQueueItemList.ascx" TagName="WorkQueueItemList" TagPrefix="localAsp" %>
    
   
<asp:UpdatePanel ID="SearchUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <ccAsp:JQuery runat="server" ID="JQuery1" MultiSelect="true" Effects="true" />
<script type="text/Javascript">
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(MultiSelect);
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(InputHover);

function MultiSelect(){
    $("#<%=TypeListBox.ClientID %>").multiSelect({
        noneSelected: '',
        oneOrMoreSelected: '% Selected',
        style: 'width: 260px'
    });
    $("#<%=StatusListBox.ClientID %>").multiSelect({
        noneSelected: '',
        oneOrMoreSelected: '% Selected',
        style: 'width: 130px;'
    });
}
</script>
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" CellSpacing="0" BorderWidth="0px">
                <asp:TableRow>
                    <asp:TableCell>                          
                                <asp:Panel ID="Panel6" runat="server" CssClass="SearchPanelContent" DefaultButton="SearchButton" >
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                         
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Patient Name" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientName" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Name" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Patient ID" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" CssClass="SearchTextBox" ToolTip="Search the list by Patient Id" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Schedule" CssClass="SearchTextBoxLabel" />&nbsp;&nbsp;
                                                <asp:LinkButton ID="ClearScheduleDateButton" runat="server" Text="X" CssClass="SmallLink"/><br />
                                                <ccUI:TextBox ID="ScheduleDate" runat="server" ReadOnly="true" CssClass="SearchDateBox" ToolTip="Search the list by Schedule Date [dd/mm/yyyy]" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="Type" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox ID="TypeListBox" runat="server" SelectionMode="Multiple" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="Status" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:ListBox ID="StatusListBox" runat="server" SelectionMode="Multiple" /></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="Priority" CssClass="SearchTextBoxLabel"
                                                    EnableViewState="False" /><br />
                                                <asp:DropDownList ID="PriorityDropDownList" runat="server" CssClass="SearchDropDownList">
                                                </asp:DropDownList>
                                            <td valign="bottom">
                                                <asp:Panel ID="Panel1" runat="server" CssClass="SearchButtonPanel"><ccUI:ToolbarButton ID="SearchButton" runat="server" SkinID="SearchIcon" OnClick="SearchButton_Click" /></asp:Panel>
                                            </td>  
                                        </tr>                                       
                                    </table>
                                </asp:Panel>
                        <ccUI:CalendarExtender ID="ScheduleCalendarExtender" runat="server" TargetControlID="ScheduleDate"
                            CssClass="Calendar">
                        </ccUI:CalendarExtender>
                    </asp:TableCell> 
                </asp:TableRow>
                     <asp:TableRow>
                        <asp:TableCell>
                        <table width="100%" cellpadding="2" cellspacing="0" class="ToolbarButtonPanel">
                            <tr><td >
                            <asp:UpdatePanel ID="ToolBarUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="ToolbarButtons" runat="server" CssClass="ToolbarButtons" style="position:relative;" >
                                        <ccUI:ToolbarButton ID="ViewItemDetailsButton" runat="server" SkinID="ViewDetailsButton" OnClick="ViewItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.View %>'/>
                                        <ccUI:ToolbarButton ID="RescheduleItemButton" runat="server" SkinID="RescheduleButton" OnClick="RescheduleItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reschedule %>'/>
                                        <ccUI:ToolbarButton ID="ResetItemButton" runat="server" SkinID="ResetButton" OnClick="ResetItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reset %>'/>
                                        <ccUI:ToolbarButton ID="DeleteItemButton" runat="server" SkinID="DeleteButton" OnClick="DeleteItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Delete %>'/>
                                        <ccUI:ToolbarButton ID="ReprocessItemButton" runat="server" SkinID="ReprocessButton" OnClick="ReprocessItemButton_Click" Roles='<%= ClearCanvas.ImageServer.Common.Authentication.AuthorityTokens.WorkQueue.Reprocess %>'/>
                                    </asp:Panel>
                             </ContentTemplate>
                          </asp:UpdatePanel>                  
                        </td></tr>
                        <tr><td>

                         <asp:Panel ID="Panel2" runat="server" style="border: solid 1px #3d98d1; ">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                 <tr><td style="border-bottom: solid 1px #3d98d1"><ccAsp:GridPager ID="GridPagerTop" runat="server" /></td></tr>                        
                                <tr><td style="background-color: white;"><localAsp:WorkQueueItemList ID="workQueueItemList" Height="500px" runat="server"></localAsp:WorkQueueItemList></td></tr>
                            </table>
                         
                        </asp:Panel>
                        </td>
                        </tr>
                        </table>
                    </asp:TableCell>
                </asp:TableRow>

            </asp:Table>

    </ContentTemplate>
</asp:UpdatePanel>

<ccAsp:MessageBox runat="server" ID="MessageBox" />
