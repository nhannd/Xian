<%@ Page Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPage.aspx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsPage" %>

<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="uc2" %>
<%@ Register Src="StudyDetailsPanel.ascx" TagName="StudyDetailsPanel" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Study Details</title>
</head>
<body style="font-family: verdana,tahoma,helvetica">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="updatepanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel runat="server" Width="100%">
                        <uc1:StudyDetailsPanel ID="StudyDetailsPanel" runat="server" />
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
