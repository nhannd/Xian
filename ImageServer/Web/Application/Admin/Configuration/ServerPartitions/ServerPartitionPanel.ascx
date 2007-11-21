<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServerPartitionPanel.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerPartitions.ServerPartitionPanel" %>
<%@ Register Src="ServerPartitionFilterPanel.ascx" TagName="ServerPartitionFilterPanel"
    TagPrefix="uc6" %>
<%@ Register Src="../../../Common/GridPager.ascx" TagName="GridPager" TagPrefix="uc1" %>
<%@ Register Src="ServerPartitionGridPanel.ascx" TagName="ServerPartitionGridPanel"
    TagPrefix="uc2" %>
<%@ Register Src="ServerPartitionToolBarPanel.ascx" TagName="ServerPartitionToolBarPanel"
    TagPrefix="uc3" %>
    
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
 <table width="100%">
        <tr>
            <td style="width: 320px">
                <uc3:ServerPartitionToolBarPanel id="ServerPartitionToolBarPanel" runat="server"></uc3:ServerPartitionToolBarPanel></td>
            <td align="right" valign="bottom">
                <uc6:ServerPartitionFilterPanel id="ServerPartitionFilterPanel" runat="server">
                </uc6:ServerPartitionFilterPanel></td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <uc2:ServerPartitionGridPanel ID="ServerPartitionGridPanel" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <uc1:GridPager id="GridPager" runat="server">
                </uc1:GridPager>
        </tr>
    </table>

    </ContentTemplate>

</asp:UpdatePanel>
