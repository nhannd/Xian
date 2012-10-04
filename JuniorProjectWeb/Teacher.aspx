<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Teacher.aspx.cs" Inherits="JuniorProjectWeb.Teacher" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>AC3 MIB - Teacher</title>
    <link href="darkbg/style.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Button ID="BtnStudent" runat="server" Height="41px" Text="Student" 
            Width="239px" onclick="BtnStudent_Click" 
            style="font-family: Calibri; font-size: large" />
        <asp:Button ID="BtnTeacher" runat="server" Height="41px" Text="Teacher" 
            Width="239px" BackColor="#282828" BorderColor="#282828" ForeColor="White" 
            style="font-family: Calibri; font-size: large" />
        <asp:Button ID="BtnAdmin" runat="server" Height="41px" Text="Admin" 
            Width="239px" onclick="BtnAdmin_Click" 
            style="font-family: Calibri; font-size: large" />
    
    </div>
    </form>
</body>
</html>