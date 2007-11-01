<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin_Configuration_EditDeviceControl" Codebehind="EditDeviceControl.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
            <asp:Panel ID="DialogPanel" runat="server"
                 CssClass="PopupWindow" Width="419px" style="display:none;">
                <asp:Label ID="TitleLabel" runat="server" CssClass="PopupWindowTitleBar" Height="28px"
                    Text="Edit Device" Width="100%"></asp:Label><br />
                <div class="PopupWindowBody" style="text-align: center">
                    <br />
                        <table id="TABLE1" runat="server" cellspacing="4" style="width: 90%; text-align: left">
                            <tr>
                                <td style="width: 162px">
                                AE Title</td>
                                <td colspan="3">
                                    <asp:TextBox ID="AETitleTextBox" runat="server" Width="100%"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 29px">
                                IP Address</td>
                                <td colspan="3" style="height: 29px">
                                    <asp:TextBox ID="IPAddressTextBox" runat="server" Width="100%"></asp:TextBox></td>
                                <td style="width: 3px; height: 29px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px">
                                Port</td>
                                <td colspan="3">
                                    <asp:TextBox ID="PortTextBox" runat="server" Width="100%"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px">
                                Description</td>
                                <td colspan="3">
                                    <asp:TextBox ID="DescriptionTextBox" runat="server" Width="100%"></asp:TextBox></td>
                                <td style="width: 3px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                Server Partition</td>
                                <td align="left" colspan="3" style="height: 24px">
                                    <asp:DropDownList ID="ServerPartitionDropDownList" runat="server" DataTextField="Description"
                                        DataValueField="Key" Width="100%">
                                    </asp:DropDownList></td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                </td>
                                <td style="width: 128px; height: 24px">
                                    <asp:CheckBox ID="ActiveCheckBox" runat="server" Text="Active" />
                                </td>
                                <td style="width: 128px; height: 24px">
                                    <asp:CheckBox ID="DHCPCheckBox" runat="server" Text="DHCP" /></td>
                                <td align="left" style="height: 24px">
                                    &nbsp;
                                </td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                </td>
                                <td style="width: 128px; height: 24px">
                                </td>
                                <td style="width: 128px; height: 24px">
                                </td>
                                <td align="left" style="height: 24px">
                                </td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 162px; height: 24px">
                                    &nbsp;</td>
                                <td style="width: 128px; height: 24px">
                                    <asp:Button ID="OKButton" runat="server" Text="Save" Width="77px" OnClick="OKButton_Click" /></td>
                                <td style="width: 128px; height: 24px">
                                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" /></td>
                                <td align="left" style="height: 24px">
                                </td>
                                <td style="width: 3px; height: 24px">
                                </td>
                            </tr>
                        </table>
                <asp:Panel ID="DummyPanel" runat="server" Height="1px" Width="303px">
                </asp:Panel>
                    &nbsp;&nbsp;</div>
            </asp:Panel>
            <cc1:ModalPopupExtender ID="ModalPopupExtender1"  runat="server" Enabled="true" TargetControlID="DummyPanel" PopupControlID="DialogPanel" >
            </cc1:ModalPopupExtender>
        <cc1:ToggleButtonExtender ID="ToggleButtonExtender1" runat="server" CheckedImageUrl="~/images/checked_small.gif"
            ImageHeight="16" ImageWidth="16" TargetControlID="ActiveCheckBox" UncheckedImageUrl="~/images/unchecked_small.gif">
        </cc1:ToggleButtonExtender>
        <cc1:ToggleButtonExtender ID="ToggleButtonExtender2" runat="server" CheckedImageUrl="~/images/checked_small.gif"
            ImageHeight="16" ImageWidth="16" TargetControlID="DHCPCheckBox" UncheckedImageUrl="~/images/unchecked_small.gif">
        </cc1:ToggleButtonExtender>
