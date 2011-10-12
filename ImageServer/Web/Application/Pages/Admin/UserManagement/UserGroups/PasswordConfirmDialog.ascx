<%--  License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordConfirmDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.PasswordConfirmDialog" %>
    <ccAsp:ModalDialog ID="ModalDialog" runat="server"></ccAsp:ModalDialog>
    
<ccAsp:ModalDialog ID="ModalDialog1" runat="server" Width="350px" Title="<%$Resources: SR, AddEditUserGroups_PasswordConfirmTitle %>">
    <ContentTemplate>
        <div class="DialogPanelContent">
            <table cellpadding="5">
                <tr>
                    <td class="DialogTextBoxLabelScroll" nowrap="nowrap" colspan="2">                    
                        <asp:TextBox ID="Description" runat="server" Text="<%$Resources: SR, AddEditUserGroups_DataAccessChanged %>" Rows="8" Wrap="true" ReadOnly="true" TextMode="MultiLine" BorderStyle="None" BorderWidth="0" CssClass="DialogTextBoxLabelScroll" Width="100%" Height="100%" />
                    </td>
                </tr>
                <tr>
                    <td class="DialogTextBoxLabel" nowrap="nowrap">
                        <asp:Label ID="Label2" runat="server" Text="<%$Resources: Labels, Password %>" CssClass="DialogTextBoxLabel" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="Password" TextMode="password" CssClass="LoginTextInput" Width="250px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                    </td>
                </tr>
            </table>
        </div>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="right">
                    <asp:Panel ID="Panel1" runat="server" CssClass="DefaultModalDialogButtonPanel">
                        <ccUI:ToolbarButton ID="OKButton" runat="server" SkinID="<%$Image:OkButton%>" OnClick="OKButton_Click" />
                        <ccUI:ToolbarButton ID="CancelButton" runat="server" SkinID="<%$Image:CancelButton%>"
                            OnClick="CancelButton_Click" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ccAsp:ModalDialog>
