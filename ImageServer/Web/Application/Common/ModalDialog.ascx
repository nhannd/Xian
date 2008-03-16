<%@ Import Namespace="System.Net.Mime" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ModalDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Common.ModalDialog" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        
            <asp:Table ID="DialogContainer" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0px" Height="100%"  Style="display: none;">
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell>
                        <asp:Panel runat="server" ID="TitleBarPanel">
                            <asp:Panel runat="server" ID="DefaultTitlePanel">
                                <asp:Table ID="Table3" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
                                    <asp:TableRow>
                                        <asp:TableCell ID="TitleBarLeft" CssClass="CSSDefaultPopupDialogTitleBarLeft">
                                        </asp:TableCell>
                                        <asp:TableCell ID="TitleBarCenter" CssClass="CSSDefaultPopupDialogTitleBarTitle">
                                            <asp:Label ID="TitleLabel" runat="server" Text="&nbsp;" ></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="TitleBarRight" VerticalAlign="middle" CssClass="CSSDefaultPopupDialogTitleBarControlBoxes">
                                            <asp:ImageButton ID="CloseButton" Style="cursor: pointer;" runat="server" ImageUrl="~/images/icons/close.png" OnClick="CloseButton_Click" />
                                        </asp:TableCell>
                                        <asp:TableCell CssClass="CSSDefaultPopupDialogTitleBarRight">
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="CustomizedTitleBarPanel">
                                <asp:PlaceHolder ID="TitlePanelPlaceHolder" runat="server"></asp:PlaceHolder>
                            </asp:Panel>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>
                        <asp:Panel runat="server" ID="ContentPanel" CssClass="CSSDefaultPopupDialogContent">
                            <asp:PlaceHolder ID="ContentPlaceHolder" runat="server"></asp:PlaceHolder>
                        </asp:Panel>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
           
        <asp:Panel ID="DummyPanel" runat="server" Height="10px" Style="left: 26px; position: absolute;
            top: -5px" Width="115px">
        </asp:Panel>
        <cc1:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="DummyPanel" BackgroundCssClass="CSSDefaultPopupDialogBacground"
            PopupControlID="DialogContainer" PopupDragHandleControlID="TitleBarPanel" RepositionMode="None" 
            >
        </cc1:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
