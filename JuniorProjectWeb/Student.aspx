<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Student.aspx.cs" Inherits="JuniorProjectWeb.Student" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>AC3 MIB - Student</title>
    <link href="darkbg/style.css" rel="Stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            font-size: xx-small;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="BtnStudent" runat="server" Height="41px" Text="Student" 
            Width="239px" BackColor="#282828" BorderColor="#282828" ForeColor="White" 
            style="font-family: Calibri; font-size: large" />
        <asp:Button ID="BtnTeacher" runat="server" Height="41px" Text="Teacher" 
            Width="239px" onclick="BtnTeacher_Click" 
            style="font-family: Calibri; font-size: large" />
        <asp:Button ID="BtnAdmin" runat="server" Height="41px" Text="Admin" 
            Width="239px" onclick="BtnAdmin_Click" 
            style="font-family: Calibri; font-size: large" />
    
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    
    </div>
    <span class="style1">Select your ID:</span>
    <br />
    <asp:DropDownList ID="ddlPatientID" runat="server" Height="24px" 
        style="font-size: medium" Width="391px" AutoPostBack="True" 
        onselectedindexchanged="ddlPatientID_SelectedIndexChanged" 
        Font-Size="XX-Large">
    </asp:DropDownList>
    &nbsp;<br />
    <br />
    <br />
    <asp:Label ID="lResult" runat="server" style="font-size: medium" Text="Result"></asp:Label>
    <br />
    <asp:Button ID="btnBurn" runat="server" Height="20px" onclick="btnBurn_Click" 
        Text="Burn" Width="182px" />
    <br />
    </form>
</body>
</html>
