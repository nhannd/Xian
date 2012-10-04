<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JuniorProjectWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Welcome to AC3 MIB!</title>
    <link href="darkbg/style.css" rel="Stylesheet" type="text/css" />
    <style type="text/css">
        .style2
        {
            font-size: xx-large;
            font-family: Arial, Helvetica, sans-serif;
        }
        .style3
        {
            color: #FFFFFF;
        }
        .style4
        {
            font-family: Arial, Helvetica, sans-serif;
        }
        .style5
        {
            color: #FFFFFF;
            font-size: medium;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" title="Welcome to AC3 MIB">
    <div title="AC3 MIB Home">
    
        <asp:Button ID="BtnStudent" runat="server" Height="41px" 
            onclick="BtnStudent_Click" style="font-family: Calibri; font-size: large" 
            Text="Student" Width="239px" />
        <asp:Button ID="BtnTeacher" runat="server" Height="41px" 
            onclick="BtnTeacher_Click" style="font-family: Calibri; font-size: large" 
            Text="Teacher" Width="239px" />
        <asp:Button ID="BtnAdmin" runat="server" Height="41px" onclick="BtnAdmin_Click" 
            style="font-family: Calibri; font-size: large" Text="Admin" Width="239px" />
    
    </div>
    </form>
    <p class="style3">
        <span class="style2">Welcome to the AC3 MIB!</p>
    </span>
        <span class="style4">
    <p class="style5">
        Select the tab that correlates to your role!</span></p>
</body>
</html>
