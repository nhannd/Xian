<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEdit.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ViewEdit" %>

<%@ Register Src="WorkQueueItemDetailsPanel.ascx" TagName="WorkQueueItemDetailsPanel"
    TagPrefix="uc2" %>

<%@ Register Src="ScheduleWorkQueueDialog.ascx" TagName="ScheduleWorkQueueDialog"
    TagPrefix="uc1" %>
<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog" TagPrefix="clearcanvas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Work Queue Item Details</title>
</head>
<body onload="self.focus();">
    <form id="form1" runat="server">
    <div style="width:1020px; height:780px">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        
        
        <uc2:WorkQueueItemDetailsPanel id="WorkQueueItemDetailsPanel1" runat="server">
        </uc2:WorkQueueItemDetailsPanel></div>
        
        <uc1:ScheduleWorkQueueDialog ID="ScheduleWorkQueueDialog1" runat="server" />
        <clearcanvas:ConfirmationDialog runat="server" ID="InformationDialog" MessageType="INFORMATION" Title=""/>
    </form>
</body>
</html>
