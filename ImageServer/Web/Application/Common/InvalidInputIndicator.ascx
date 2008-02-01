<%@ Control Language="C#" AutoEventWireup="true" Codebehind="InvalidInputIndicator.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.InvalidInputIndicator" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="ContainerPanel" runat="server" CssClass="CSSInvalidInputIndicatorContainer">
    <asp:Image ID="Image" runat="server" />
    <asp:Panel ID="HintPanel" runat="server" Wrap="true" CssClass="CSSInvalidIndicatorTooltipPanel">
        <asp:Label ID="HintLabel" runat="server" Text="Label" CssClass="CSSInvalidIndicatorTooltip"></asp:Label>
    </asp:Panel>
    <cc1:HoverMenuExtender ID="HoverMenuExtender1" runat="server" OffsetX="20" OffsetY="20"
        PopDelay="100" PopupControlID="HintPanel" TargetControlID="Image">
    </cc1:HoverMenuExtender>
</asp:Panel>
