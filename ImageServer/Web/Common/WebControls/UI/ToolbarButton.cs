#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Common.Utilities;


[assembly: WebResource("ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js", "text/javascript")]

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    public enum NoPermissionVisibility
    {
        Visible,
        Invisible
    }

    [ToolboxData("<{0}:ToolbarButton runat=server></{0}:ToolbarButton>")]
    [Themeable(true)]
    public class ToolbarButton : ImageButton, IScriptControl
    {
        #region Private Members
        private string _roleSeparator = ",";
        private NoPermissionVisibility _noPermissionVisibilityMode;
        #endregion

        #region Public Properties

        /// <summary>
		/// Specifies the roles which users must have to access to this button.
		/// </summary>
        public string Roles
        {
            get
            {
                return ViewState["Roles"] as string;
            }
            set
            {
                ViewState["Roles"] = value;
            }
        }

		/// <summary>
		/// Specifies the visiblity of the button if the user doesn't have the roles specified in <see cref="Roles"/>
		/// </summary>
        public NoPermissionVisibility NoPermissionVisibilityMode
        {
            get { return _noPermissionVisibilityMode; }
            set { _noPermissionVisibilityMode = value; }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is enabled.
        /// </summary>
        public string EnabledImageURL 
        {
            get
            {
                String s = (String)ViewState["EnabledImageURL"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["EnabledImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the button enabled and user hovers the mouse over the button.
        /// </summary>
        public string HoverImageURL
        {
            get
            {
                String s = (String)ViewState["HoverImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["HoverImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Sets or gets the url of the image to be used when the mouse button is clicked.
        /// </summary>
        public string ClickedImageURL
        {
            get
            {
                String s = (String)ViewState["ClickedImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["ClickedImageURL"] = inspectURL(value);
            }
        }   

        /// <summary>
        /// Sets or gets the url of the image to be used when the button is disabled.
        /// </summary>
        public string DisabledImageURL
        {
            get
            {
                String s = (String)ViewState["DisabledImageURL"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["DisabledImageURL"] = inspectURL(value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to seperate values in the <see cref="Roles"/> property.
        /// </summary>
        public string RoleSeparator
        {
            get { return _roleSeparator; }
            set { _roleSeparator = value; }
        }

        #endregion Public Properties

        #region Private Methods

        private string inspectURL(string value)
        {
            if (!value.StartsWith("~/") && !value.StartsWith("/")) 
                value = value.Insert(0, "~/App_Themes/" + Page.Theme + "/");
            
            return value;
        }       

        #endregion Private Methods


        public override void  RenderControl(HtmlTextWriter writer)
        {
            if (Enabled)
                ImageUrl = EnabledImageURL;
            else
                ImageUrl = DisabledImageURL;

 	        base.RenderControl(writer);
        }


        #region IScriptControl Members
        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            ScriptControlDescriptor desc = new ScriptControlDescriptor("ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton", ClientID);
            desc.AddProperty("EnabledImageUrl", Page.ResolveClientUrl(EnabledImageURL));
            desc.AddProperty("DisabledImageUrl", Page.ResolveClientUrl(DisabledImageURL));
            desc.AddProperty("HoverImageUrl", Page.ResolveClientUrl(HoverImageURL));
            desc.AddProperty("ClickedImageUrl", Page.ResolveClientUrl(ClickedImageURL));

            return new ScriptDescriptor[] { desc };
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference();

            reference.Path = Page.ClientScript.GetWebResourceUrl(typeof(ToolbarButton), "ClearCanvas.ImageServer.Web.Common.WebControls.UI.ToolbarButton.js");
            return new ScriptReference[] { reference };
        }

        #endregion IScriptControl Members

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptControl(this);
            }

            if (String.IsNullOrEmpty(Roles)==false)
            {
                string[] roles = Roles.Split(new String[]{ RoleSeparator}, StringSplitOptions.RemoveEmptyEntries);
                bool allow = CollectionUtils.Contains(roles,
                                delegate(string role)
                                 {
                                     return Thread.CurrentPrincipal.IsInRole(role.Trim());
                                 });

                if (!allow)
                {
                    Enabled = false;
                    Visible = NoPermissionVisibilityMode==NoPermissionVisibility.Invisible;
                }
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!DesignMode)
            {
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                sm.RegisterScriptDescriptors(this);
            }
            base.Render(writer);
        }
       
    }
}
