<%-- 

This control has been downloaded from CodeProject @ http://www.codeproject.com/KB/user-controls/CheckJavascriptEnabled.aspx 
Include the control in the MasterPage, and provide a URL for javascript disabled browsers.
    
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckJavascript.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.CheckJavascript" %>

<asp:HiddenField ID="hfClientJSEnabled" runat="server" Value="False" />

<script type="text/javascript">
    document.getElementById('<%= hfClientJSEnabled.ClientID %>').value = "True";
    if (document.getElementById('<%= hfClientJSEnabled.ClientID %>').value != '<%= IsJSEnabled %>')
    {        
        window.location.href= '<%= GetAppendedUrl(JSQRYPARAM, JSENABLED) %>';
    }
</script>

