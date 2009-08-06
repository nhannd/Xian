<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSeriesDelete.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Test.TestSeriesDelete" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Server AE
        <asp:TextBox runat="server" ID="ServerAE"></asp:TextBox>
        Study Instance UID
        <asp:TextBox runat="server" ID="StudyUID" Width="200px"></asp:TextBox>
        Series Instance UID(s)
        <asp:TextBox runat="server" ID="SeriesUID" Width="200px"></asp:TextBox>
        <asp:Button runat="server" ID="Delete" Text="Delete" OnClick="DeleteClick" />
    </div>
    </form>
</body>
</html>
