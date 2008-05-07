<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SeriesDetailsPage.aspx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.Pages.SeriesDetails.SeriesDetailsPage" %>

<%@ Register Src="SeriesDetailsPanel.ascx" TagName="SeriesDetailsPanel" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Series Details</title>
    
</head>
<body style="font-family: verdana,tahoma,helvetica">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            
            <asp:UpdatePanel runat="server" ID="updatepanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc1:SeriesDetailsPanel id="SeriesDetailsPanel1" runat="server">
                    </uc1:SeriesDetailsPanel>
                    
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
