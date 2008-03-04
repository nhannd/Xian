<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Common.SectionPanel" %>
<asp:Panel ID="Container" runat="server">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <th align="left">
                <asp:Panel ID="HeadingPanel" runat="server" width="100%" Wrap="false">
                    <asp:Label ID="HeadingTextLabel" runat="server" Text="Label"></asp:Label></asp:Panel>
            </th>        
        </tr>
        <tr>
            <td>
                <asp:Panel ID="ContentPanel" runat="server" width="100%">
                    <asp:PlaceHolder ID="placeholder"  runat="server"></asp:PlaceHolder>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>
