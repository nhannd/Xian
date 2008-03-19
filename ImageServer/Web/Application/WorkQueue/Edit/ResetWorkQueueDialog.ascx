<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResetWorkQueueDialog.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ResetWorkQueueDialog" %>
<%@ Register Src="~/Common/ModalDialog.ascx" TagName="ModalDialog" TagPrefix="uc1" %>

<%@ Register Src="~/Common/ConfirmationDialog.ascx" TagName="ConfirmationDialog"
    TagPrefix="clearcanvas" %>

<clearcanvas:ConfirmationDialog ID="PreResetConfirmDialog" runat="server" >
</clearcanvas:ConfirmationDialog>

<clearcanvas:ConfirmationDialog ID="ConfirmationDialog" runat="server" MessageType="INFORMATION" >
</clearcanvas:ConfirmationDialog>


