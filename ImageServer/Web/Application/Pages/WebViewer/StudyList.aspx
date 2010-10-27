<!--
// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.
-->
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudyList.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.WebViewer.StudyList"%>

<%@ Register Src="StudyListGridView.ascx" TagName="StudyListGridView" TagPrefix="localAsp" %>
<%@ Register Src="SearchPanel.ascx" TagName="SearchPanel" TagPrefix="localAsp" %>
<%@ Register Src="SessionTimeout.ascx" TagName="SessionTimeout" TagPrefix="ccAsp" %>
<%@ Register Src="JQuery.ascx" TagName="JQuery" TagPrefix="localAsp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <localAsp:JQuery ID="MainJQuery" MultiSelect="true" Effects="true" runat="server" />
</head>
<body style="background: #333333; margin: 0px;">
    <form id="form1" runat="server">
    
        <asp:ScriptManager ID="GlobalScriptManager" runat="server" EnableScriptGlobalization="true"
            EnableScriptLocalization="true" OnAsyncPostBackError="GlobalScriptManager_AsyncPostBackError" >
        </asp:ScriptManager>   
        
        <ccAsp:SessionTimeout runat="server" ID="SessionTimeout" />
   
        <div>
            <div><table width="100%" cellpadding="0" cellspacing="0"><tr><td style="padding-bottom: 2px"><asp:Image ID="Image1" ImageUrl="~/Pages/WebViewer/Images/StudiesPageLogo.png" runat="server" /></td><td valign="bottom" align="right" style="padding-bottom: 2px; padding-right: 5px;"><span style="color: #dddddd; font-family: Sans-Serif; font-weight: bold">Multiple matching studies have been found. Please select a study to view.</span></td></tr></table></div>
            <asp:Panel ID="Panel2" runat="server" style="border-top: solid 2px #999999;">
                <localAsp:SearchPanel ID="SearchPanel" runat="server"></localAsp:SearchPanel>
            </asp:Panel>
    </div>
    </form>
</body>
</html>
