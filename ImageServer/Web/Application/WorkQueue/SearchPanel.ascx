<%@ Import Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SearchPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.SearchPanel" %>
<%@ Register Src="WorkQueueItemListPanel.ascx" TagName="WorkQueueSearchResultPanel" TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/GridPager.ascx" TagName="GridPager" TagPrefix="clearcanvas" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI" 
    Assembly="ClearCanvas.ImageServer.Web.Common" %>
    

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxControlToolkit" %>

<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="PagePanel" runat="server">
            <asp:Table ID="Table" runat="server" Width="100%" CellPadding="0" BorderWidth="0px">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell HorizontalAlign="left" Width="100%">
                        <asp:Panel ID="Panel2" runat="server" CssClass="CSSToolbarPanelContainer">
                                <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                    <clearcanvas:ToolbarButton
                                            ID="ViewToolbarButton1" runat="server" 
                                            EnabledImageURL="~/images/icons/OpenEnabled.png" 
                                            DisabledImageURL="~/images/icons/OpenDisabled.png"
                                            OnClick="ViewButton_Click" AlternateText="View Details" 
                                            />
                                     <clearcanvas:ToolbarButton
                                            ID="RescheduleToolbarButton" runat="server" 
                                            EnabledImageURL="~/images/icons/ScheduleEnabled.png" 
                                            DisabledImageURL="~/images/icons/ScheduleDisabled.png"
                                            OnClick="RescheduleButton_Click" AlternateText="Reschedule" 
                                            />
                                      <clearcanvas:ToolbarButton ID="ResetButton" runat="server" EnabledImageURL="~/images/icons/ResetEnabled.png"
                                        DisabledImageURL="~/images/icons/ResetDisabled.png" AlternateText="Reset this item" OnClick="Reset_Click"/>
                                    
                                      <clearcanvas:ToolbarButton ID="DeleteButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                                        DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this item" OnClick="Delete_Click"/>
                                    
                                    </asp:Panel>
                                </asp:Panel>
                            
                    </asp:TableHeaderCell>
                    <asp:TableHeaderCell HorizontalAlign="right" VerticalAlign="Bottom">
                        <asp:Panel ID="FilterPanel" runat="server" CssClass="CSSFilterPanelContainer">
                            <asp:Panel ID="Panel5" runat="server" CssClass="CSSFilterPanelBorder">
                                <asp:Panel ID="Panel6" runat="server" CssClass="CSSFilterPanelContent">
                                    <table cellpadding="3" cellspacing="0">
                                        <tr>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label2" runat="server" Text="Patient ID" Width="100px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="PatientId" runat="server" Width="100px" ToolTip="Filter the list by Patient Id" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label3" runat="server" Text="Accession#" Width="100px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="AccessionNumber" runat="server" Width="100px" ToolTip="Filter the list by Accession Number" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label4" runat="server" Text="Description" Width="100px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:TextBox ID="StudyDescription" runat="server" Width="100px" ToolTip="Filter the list by Study Description" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label1" runat="server" Text="Schedule" Style="padding-right: 5px" />
                                                <asp:ImageButton ID="ClearScheduleDateButton" runat="server" ImageUrl="~/images/icons/EraserEnabled.png" />
                                                <asp:TextBox ID="ScheduleDate" runat="server" Width="100px" ReadOnly="true" ToolTip="Filter the list by Scheduled Date [dd/mm/yyyy]" />
                                            </td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label5" runat="server" Text="Type" Width="68px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:DropDownList ID="TypeDropDownList" runat="server" Width="120px">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label6" runat="server" Text="Status" Width="68px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:DropDownList ID="StatusDropDownList" runat="server" Width="120px">
                                                </asp:DropDownList></td>
                                            <td align="left" valign="bottom">
                                                <asp:Label ID="Label7" runat="server" Text="Priority" Width="68px" Style="padding-right: 5px"
                                                    EnableViewState="False" /><br />
                                                <asp:DropDownList ID="PriorityDropDownList" runat="server" Width="120px">
                                                </asp:DropDownList></td>
                                            <td align="right" valign="bottom" style="width: 57px">
                                                <asp:ImageButton ID="FilterButton" runat="server" ImageUrl="~/images/icons/QueryEnabled.png"
                                                    OnClick="FilterButton_Click" ToolTip="Filter" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                        <AjaxControlToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="ScheduleDate"
                            CssClass="Calendar">
                        </AjaxControlToolkit:CalendarExtender>
                    </asp:TableHeaderCell>
                </asp:TableHeaderRow>
                <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <asp:Panel runat="server" CssClass="CSSGridViewPanelContainer">
                                <asp:Panel runat="server" CssClass="CSSGridViewPanelBorder" >
                                    <clearcanvas:WorkQueueSearchResultPanel ID="workQueueItemListPanel" Height="500px" AllowPaging="true" PageSize="20" runat="server"></clearcanvas:WorkQueueSearchResultPanel>
                                </asp:Panel>                        
                        </asp:Panel>
                        
                    </asp:TableCell>
                </asp:TableRow>
                 <asp:TableRow>
                    <asp:TableCell ColumnSpan="2">
                        <clearcanvas:GridPager ID="GridPager1" runat="server"></clearcanvas:GridPager>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<clearcanvas:ConfirmationDialog runat="server" ID="ConfirmationDialog" />
