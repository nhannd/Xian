<%@ Control Language="C#" AutoEventWireup="true" Codebehind="DeleteWorkQueueDialog.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.DeleteWorkQueueDialog" %>

<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog"
    TagPrefix="clearcanvas" %>

<clearcanvas:ConfirmationDialog ID="PreDeleteConfirmDialog" runat="server"></clearcanvas:ConfirmationDialog>
<clearcanvas:ConfirmationDialog ID="ConfirmationDialog" runat="server" MessageType="INFORMATION">
</clearcanvas:ConfirmationDialog>
