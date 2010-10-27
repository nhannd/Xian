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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" MasterPageFile="ErrorPageMaster.Master" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Error.ErrorPage" %>
<%@ Import namespace="System.Threading"%>

<asp:Content runat="server" ContentPlaceHolderID="ErrorMessagePlaceHolder">
	    <asp:label ID="ErrorMessageLabel" Text="Something happened that the ImageServer was unprepared for." runat="server" />
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="DescriptionPlaceHolder">

    <asp:Label ID = "DescriptionLabel" runat="server">
        This message is being displayed because the ClearCanvas ImageServer encountered a 
        situation that was unexpected. The resulting error has been recorded for future analysis.
         <br/><br/>
         If you would like to report the error, please post to one of the forums listed below and 
         provide any information you think will be helpful in handling this situation in the future. 
         <asp:panel ID="StackTraceMessage" runat="server" Visible="false">To include the ImageServer's error message, <a class="ErrorLink" href="javascript:toggleLayer('StackTrace');">click here</a>.</asp:panel>
    </asp:Label>
    <div id="StackTrace" style="margin-top: 15px" visible="false"><asp:TextBox runat="server" ID="StackTraceTextBox" Visible="false" Rows="5" Columns="57" TextMode="MultiLine" ReadOnly="true"></asp:TextBox></div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="UserEscapePlaceHolder">
    <table width="100%" class="UserEscapeTable"><tr><td class="UserEscapeCell"><a href="javascript:history.back()" class="UserEscapeLink">Back</a></td><td class="UserEscapeCell"><asp:LinkButton ID="LogoutButton" runat="server" CssClass="UserEscapeLink" OnClick="Logout_Click" Text="Logout"></asp:LinkButton></td><td style="width: 30%" align="center"><a href="javascript:window.close()" class="UserEscapeLink">Close</a></td></tr></table>
</asp:Content>