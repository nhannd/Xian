<%@ Page Language="C#" AutoEventWireup="true" Codebehind="ViewEdit.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ViewEdit" %>

<%@ Register Src="WorkQueueItemDetailsPanel.ascx" TagName="WorkQueueItemDetailsPanel"
    TagPrefix="uc2" %>
<%@ Register Src="ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog"
    TagPrefix="uc1" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog"
    TagPrefix="clearcanvas" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Work Queue Item Details</title>
</head>
<body onload="self.focus();">
    <form id="form1" runat="server">
        <div style="width: 1020px; height: 780px">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc2:WorkQueueItemDetailsPanel ID="WorkQueueItemDetailsPanel1" runat="server"></uc2:WorkQueueItemDetailsPanel>
                    <uc1:ScheduleWorkQueueDialog ID="ScheduleWorkQueueDialog1" runat="server" />
                    <clearcanvas:ConfirmationDialog runat="server" ID="InformationDialog" MessageType="INFORMATION"
                        Title="" />
                    <center>
                        <asp:Label ID="Message" runat="server" Text="Label"></asp:Label>
                    </center>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
