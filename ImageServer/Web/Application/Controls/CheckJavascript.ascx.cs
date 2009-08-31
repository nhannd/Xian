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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class CheckJavascript : UserControl
    {
        protected static string JSDISABLED = "0";
        protected static string JSENABLED = "1";
        protected static string JSQRYPARAM = "jse";

        protected bool IsJSEnabled
        {
            get
            {
                if (Session["JS"] == null)
                    Session["JS"] = true;

                return (bool) Session["JS"];
            }
            set { Session["JS"] = value; }
        }

        protected string JSTargetURL
        {
            get { return Request.Url.ToString(); }
        }

        protected string NonJSTargetURL
        {
            get { return ResolveServerUrl(ImageServerConstants.PageURLs.JavascriptErrorPage, false); }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Request.QueryString[JSQRYPARAM] != null)
            {
                IsJSEnabled = Request.QueryString[JSQRYPARAM] == JSENABLED;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected string GetAppendedUrl(string newParam, string newParamValue)
        {
            string targeturl = string.Empty;
            Uri url = (string.IsNullOrEmpty(NonJSTargetURL))
                          ? new Uri(JSTargetURL)
                          : new Uri(NonJSTargetURL);
            if (url == null)
                url = Request.Url;

            string[] qry = url.Query.Replace("?", "").Split('&');

            StringBuilder sb = new StringBuilder();
            foreach (string s in qry)
            {
                if (!s.ToLower().Contains(newParam.ToLower()))
                {
                    sb.Append(s + "&");
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                targeturl = string.Format("{0}?{1}&{2}={3}", url.AbsolutePath, sb, newParam, newParamValue);
            }
            else
            {
                targeturl = string.Format("{0}?{1}={2}", url.AbsolutePath, newParam, newParamValue);
            }
            return targeturl;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (IsJSEnabled)
            {
                string targeturl = GetAppendedUrl(JSQRYPARAM, JSDISABLED);
                HtmlGenericControl ctrl = new HtmlGenericControl("NOSCRIPT");
                ctrl.InnerHtml = string.Format("<meta http-equiv=REFRESH content=0;URL={0}>", targeturl);
                Page.Header.Controls.Add(ctrl);
            }
            else
            {
                if (!string.IsNullOrEmpty(NonJSTargetURL))
                    Response.Redirect(NonJSTargetURL);
                HtmlGenericControl ctrl = new HtmlGenericControl("NOSCRIPT");
                ctrl.InnerHtml = string.Empty;
                Page.Header.Controls.Add(ctrl);
            }
        }

        public string ResolveServerUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)

                return serverUrl;
            string newUrl = ResolveUrl(serverUrl);
            Uri originalUri = HttpContext.Current.Request.Url;

            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                     "://" + originalUri.Authority + newUrl;
            return newUrl;
        }
    }

    public class CheckJavaScriptHelper
    {
        public static bool IsJavascriptEnabled
        {
            get
            {
                if (HttpContext.Current.Session["JS"] == null)
                    HttpContext.Current.Session["JS"] = true;

                return (bool) HttpContext.Current.Session["JS"];
            }
        }
    }
}