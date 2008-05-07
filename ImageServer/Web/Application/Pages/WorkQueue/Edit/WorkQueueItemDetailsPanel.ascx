<%@ Control Language="C#" AutoEventWireup="true" Codebehind="WorkQueueItemDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit.WorkQueueItemDetailsPanel" %>

<asp:Panel ID="Panel1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="conditional">
        <ContentTemplate>
            <asp:Panel runat="server" ID="WorkQueueDetailsPanelContainer">
                <ccAsp:SectionPanel ID="WorkQueueDetailSectionPanel" runat="server" HeadingText="Work Queue Item Details"
                    HeadingCSS="CSSWorkQueueDetailSectionHeading" Width="100%" CssClass="CSSSection">
                    <SectionContentTemplate>
                        <asp:Panel ID="Panel6" runat="server" CssClass="CSSToolbarPanelContainer">
                            <asp:Panel ID="Panel7" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                                <asp:Panel ID="Panel8" runat="server" CssClass="CSSToolbarContent">
                                    <ccUI:ToolbarButton ID="RescheduleToolbarButton" runat="server" EnabledImageURL="~/images/icons/ScheduleEnabled.png"
                                        DisabledImageURL="~/images/icons/ScheduleDisabled.png" AlternateText="Reschedule this item"
                                        OnClick="Reschedule_Click" />
                                    <ccUI:ToolbarButton ID="ResetButton" runat="server" EnabledImageURL="~/images/icons/ResetEnabled.png"
                                        DisabledImageURL="~/images/icons/ResetDisabled.png" AlternateText="Reset this item"
                                        OnClick="Reset_Click" />
                                    <ccUI:ToolbarButton ID="DeleteButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                                        DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this item"
                                        OnClick="Delete_Click" />
                                </asp:Panel>
                            </asp:Panel>
                        </asp:Panel>
                        <asp:PlaceHolder ID="WorkQueueDetailsViewPlaceHolder" runat="server"></asp:PlaceHolder>
                    </SectionContentTemplate>
                </ccAsp:SectionPanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Timer ID="RefreshTimer" runat="server" Interval="10000" OnTick="RefreshTimer_Tick">
    </asp:Timer>
</asp:Panel>
