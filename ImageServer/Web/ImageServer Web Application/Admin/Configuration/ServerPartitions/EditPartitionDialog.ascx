<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.ServerPartitions.EditPartitionDialog" Codebehind="EditPartitionDialog.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:UpdatePanel ID="UpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Panel ID="DialogPanel" runat="server"
                 CssClass="PopupWindow" Width="400px" style="display:none;">
                <asp:Panel ID="TitleBarPanel" runat="server" CssClass="PopupWindowTitleBar"
                    Width="100%">
                    <table style="width: 100%">
                        <tr>
                            <td valign="middle">
                <asp:Label ID="TitleLabel" runat="server" Text="Edit Partition" Width="100%"></asp:Label></td>
                            
                        </tr>
                    </table>
                </asp:Panel>
                
                <div class="PopupWindowBody">
                    <br />
                        <table id="TABLE1" cellspacing="4"  runat="server"  style="width: 90%;">
                            <tr>
                                <td >
                                AE Title</td>
                                <td colspan="2">
                                    <asp:TextBox ID="AETitleTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td >
                                Port</td>
                                <td colspan="2">
                                    <asp:TextBox ID="PortTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td >
                                Description</td>
                                <td colspan="2">
                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td style="height: 24px">
                                    Folder</td>
                                <td align="left" colspan="2" style="height: 24px">
                                    <asp:TextBox ID="FolderTextBox" runat="server" Width="100%" BorderColor="LightSteelBlue" BorderWidth="1px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td >
                                </td>
                                <td >
                                    <asp:CheckBox ID="EnabledCheckBox" runat="server" Text="Enabled" />
                                </td>
                                <td >
                                    </td>
                            </tr>
                            <tr>
                                <td style="height: 24px">
                                </td>
                                <td style="height: 24px">
                                </td>
                                <td style="height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 24px">
                                    </td>
                                <td style="height: 24px">
                                    <asp:Button ID="OKButton" runat="server" Text="Save" Width="77px" OnClick="OKButton_Click" /></td>
                                <td style="height: 24px">
                                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                            </tr>
                        </table>
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="303px">
                </asp:Panel></div>
            </asp:Panel>
        <cc1:ToggleButtonExtender ID="ToggleButtonExtender2" runat="server" CheckedImageUrl="~/images/checked_tall.gif"
            ImageHeight="16" ImageWidth="12" TargetControlID="EnabledCheckBox" UncheckedImageUrl="~/images/unchecked_tall.gif">
        </cc1:ToggleButtonExtender>
            <cc1:ModalPopupExtender ID="ModalPopupExtender1"  runat="server" Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel"  BackgroundCssClass ="modalBackground">
            </cc1:ModalPopupExtender>
    </ContentTemplate>
</asp:UpdatePanel>
