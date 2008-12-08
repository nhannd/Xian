<%@ Import Namespace="System.Net.Mime" %>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ModalDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Controls.ModalDialog" %>

<asp:UpdatePanel ID="ModalDialogUpdatePanel" runat="server" UpdateMode="Conditional" >
    <ContentTemplate>
    
        <!-- Dialog Box -->
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Table ID="DialogContainer" runat="server" CellPadding="0" CellSpacing="0" BorderWidth="0px"
                    Height="100%" Style="display: none;">
                    <asp:TableRow ID="TableRow1" runat="server">
                        <asp:TableCell>
                            <asp:Panel runat="server" ID="TitleBarPanel">
                                <asp:Panel runat="server" ID="DefaultTitlePanel">
                                    <asp:Table ID="Table3" runat="server" CellPadding="0" CellSpacing="0" Width="100%">
                                        <asp:TableRow>
                                            <asp:TableCell ID="TitleBarLeft" CssClass="DefaultModalDialogTitleBarLeft">
                                            </asp:TableCell>
                                            <asp:TableCell ID="TitleBarCenter" CssClass="DefaultModalDialogTitleBarTitle">
                                                    <asp:Panel runat="server" style="padding-bottom: 4px">
                                                        <asp:Label ID="TitleLabel" runat="server" Text="&nbsp;"></asp:Label>
                                                    </asp:Panel>
                                            </asp:TableCell>
                                            <asp:TableCell CssClass="DefaultModalDialogTitleBarRight">
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
                            <asp:Panel runat="server" ID="ContentPanel" CssClass="DefaultModalDialogContent">
                                <asp:PlaceHolder ID="ContentPlaceHolder" runat="server"></asp:PlaceHolder>
                            </asp:Panel>
                        </asp:TableCell>
                    </asp:TableRow>

                </asp:Table>

            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Covers the entire background and prevents the user from clicking on the screen outside of
             the dialog box -->
        <asp:Panel ID="DummyPanel" runat="server" Height="10px" Style="left: 26px; position: absolute;
            top: -5px" Width="115px">
        </asp:Panel>
        <aspAjax:ModalPopupExtender ID="ModalPopupExtender" runat="server" TargetControlID="DummyPanel" 
            PopupControlID="DialogContainer" PopupDragHandleControlID="TitleBarPanel" RepositionMode="None">
        </aspAjax:ModalPopupExtender>
        


    </ContentTemplate>
</asp:UpdatePanel>
