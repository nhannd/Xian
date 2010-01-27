#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

#endregion

using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Exceptions;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class GlobalMasterPage : MasterPage, IMasterProperties
    {
        private bool _displayUserInfo = true;

        public bool DisplayUserInformationPanel
        {
            get { return _displayUserInfo; }
            set { _displayUserInfo = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            if (ConfigurationManager.AppSettings.GetValues("CachePages")[0].Equals("false"))
            {
                Response.CacheControl = "no-cache";
                Response.AddHeader("Pragma", "no-cache");
                Response.Expires = -1;
            }

            AddIE6PngBugFixCSS();

            CustomIdentity id = SessionManager.Current.User.Identity as CustomIdentity;

            if (DisplayUserInformationPanel)
            {
                if (id != null)
                {
                    Username.Text = id.DisplayName;
                }
                else
                {
                    Username.Text = "unknown";
                }

                try
                {
                    AlertIndicator alertControl = (AlertIndicator)LoadControl("~/Controls/AlertIndicator.ascx");
                    AlertIndicatorPlaceHolder.Controls.Add(alertControl);
                }
                catch (Exception)
                {
                    //No permissions for Alerts, control won't be displayed
                    //hide table cell that contains the control.
                    AlertIndicatorCell.Visible = false;
                }
            }
            else
            {
                UserInformationCell.Width = Unit.Percentage(0);
                MenuCell.Width = Unit.Percentage(100);
            }
        }

        private void AddIE6PngBugFixCSS()
        {
            IE6PNGBugFixCSS.InnerHtml = @"
            input, img
            {
                background-image: expression
                (
                        this.src.toLowerCase().indexOf('.png')>-1?
                        (
                            this.runtimeStyle.backgroundImage = ""none"",
                            this.runtimeStyle.filter = ""progid:DXImageTransform.Microsoft.AlphaImageLoader(src='"" + this.src + ""', sizingMethod='image')"",
                            this.src = """ + Page.ResolveClientUrl("~/App_Themes/Default/Images/blank.gif") + @"""
                        )
                        
                );
            }
        ";
        }

        protected void Logout_Click(Object sender, EventArgs e)
        {
            Platform.Log(LogLevel.Info, "{0} has logged out.", SessionManager.Current.User.Identity.Name);
            SessionManager.SignOut();
            Response.Redirect(SessionManager.LoginUrl);
        }

        protected void GlobalScriptManager_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            GlobalScriptManager.AsyncPostBackErrorMessage = ExceptionHandler.ThrowAJAXException(e.Exception);
        }
    }
}