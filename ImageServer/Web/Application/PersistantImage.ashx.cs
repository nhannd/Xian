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
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class PersistantImage : IHttpHandler
    {
        private HttpContext _currentContext = null;

        protected string Key
        {
            get
            {
                return ((_currentContext != null) && (_currentContext.Request != null) && (_currentContext.Request.QueryString["key"] != null)) ? _currentContext.Request.QueryString["key"] : "";
            }
        }

        protected string Url
        {
            get
            {
                return ((Key.Length > 0) && (WebConfigurationManager.AppSettings.Get(Key) != null)) ? WebConfigurationManager.AppSettings.Get(Key) : "";
            }
        }

        protected string Path
        {
            get
            {
                return (Url.Length > 0) ? HttpContext.Current.Server.MapPath("~/App_Themes/" + ImageServerConstants.Theme + "/" + Url) : "";
            }
        }

        protected string Extension
        {
            get
            {
                return ((Path.Length > 0) && (Path.LastIndexOf('.') > -1) && (Path.LastIndexOf('.') < (Path.Length - 1))) ? Path.Substring(Path.LastIndexOf('.') + 1).ToLower() : "";
            }
        }

        protected bool IsSafe()
        {
            bool bSafeExtension = (Extension.Equals("jpg") || Extension.Equals("jpeg") || Extension.Equals("gif") || Extension.Equals("png"));

            Regex regEx = new Regex("[^a-zA-Z0-9._-~/]");
            bool bSafeChars = (!regEx.IsMatch(Url)) && (!Url.Contains(".."));

            return (bSafeExtension && bSafeChars);
        }

        public void ProcessRequest(HttpContext context)
        {
            _currentContext = context;

            if ((Path.Length > 0) && IsSafe())
            {
                context.Response.ContentType = "image/" + Extension;
                context.Response.Cache.AppendCacheExtension("post-check=900,pre-check=3600");
                context.Response.WriteFile(Path);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("");
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }

}