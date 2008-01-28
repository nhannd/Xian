<%@ Control Language="C#" AutoEventWireup="true" Codebehind="InvalidInputIndicator.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.InvalidInputIndicator" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Panel ID="ContainerPanel" runat="server" Wrap="False">
    <asp:Image ID="Image" runat="server" />
    
    <asp:Panel ID="HintPanel" runat="server" Style="color: #ff0066">
    <asp:Label ID="HintLabel" runat="server" BorderColor="Silver" BorderStyle="Solid"
        BorderWidth="1px" Style="position:relative; padding-right: 10px; padding-left: 10px; padding-bottom: 5px;
        color: #ff6600; padding-top: 5px; background-color: #ffffcc" Text="Label"></asp:Label>
        
        </asp:Panel>
        
<cc1:HoverMenuExtender ID="HoverMenuExtender1" runat="server" OffsetX="20" OffsetY="20"
    PopDelay="100" PopupControlID="HintPanel" TargetControlID="Image"></cc1:HoverMenuExtender>

</asp:Panel>

