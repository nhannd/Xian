<%@ Control Language="C#" AutoEventWireup="true" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServiceLocks.AddEditServiceLockDialog"
    Codebehind="AddEditServiceLockDialog.ascx.cs" %>
<%@ Register Src="~/Common/InvalidInputIndicator.ascx" TagName="InvalidInputIndicator"
    TagPrefix="CCCommon" %>
<%@ Register Src="~/Common/ModalDialog.ascx" TagName="ModalDialog" TagPrefix="clearcanvas" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.Validators"
    TagPrefix="CCValidators" %>
<%@ Register Assembly="ClearCanvas.ImageServer.Web.Common" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    TagPrefix="clearcanvas" %>
<clearcanvas:ModalDialog ID="ModalDialog1" runat="server" Title="Edit Service Lock" BackgroundCSS="CSSModalBackground"
    Width="450px">
    <ContentTemplate>
        <asp:Panel ID="Panel3" runat="server" style="border-width:1px; border-color:#b0c4de; border-style:solid; padding:10px;">
            <asp:Table ID="Table1" runat="server" CellSpacing="3">
                <asp:TableRow>
                    <asp:TableCell Width="30%">
                        Description
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Description" runat="server" Text="Label"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell>
                        Type
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="Type" runat="server" Text="Label"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                
                <asp:TableRow>
                    <asp:TableCell>
                        File System
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:Label ID="FileSystem" runat="server" Text="Label"></asp:Label>           
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell>
                        Enabled
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:CheckBox ID="Enabled" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                
                <asp:TableRow>
                    <asp:TableCell>
                        Schedule
                    </asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="ScheduleDate" runat="server" Width="80px"></asp:TextBox>
                        <asp:Button ID="DatePickerButton" runat="server" Text="..." />
                        <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="ScheduleDate" PopupButtonID="DatePickerButton" CssClass="Calendar">
                        </ajaxToolkit:CalendarExtender>&nbsp;
                        <asp:DropDownList ID="ScheduleTimeDropDownList" runat="server"></asp:DropDownList>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
        
        <center>
                <br />
                <table width="80%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="OKButton" runat="server" Text="Apply" Width="77px" OnClick="ApplyButton_Click"
                                ValidationGroup="vg1" />
                        </td>
                        <td align="center">
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
                <br />
            </center>
    </ContentTemplate>
</clearcanvas:ModalDialog>

<clearcanvas:ConfirmationDialog ID="MessageBox" runat="server" />
