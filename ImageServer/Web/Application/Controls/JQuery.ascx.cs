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

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class JQuery : System.Web.UI.UserControl
    {
        private bool _multiselect = false;
    	private bool _maskedinput = false;

        public bool MultiSelect
        {
            get { return _multiselect;  }
            set { _multiselect = value; }
        }

		public bool MaskedInput
		{
			get { return _maskedinput; }
			set { _maskedinput = value; }
		}
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "jQuery", ResolveUrl("~/Scripts/jquery/jquery-1.3.2.min.js"));

            //Default Libraries
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "ClearCanvas", ResolveUrl("~/Scripts/ClearCanvas.js"));
            Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "DropShadow", ResolveUrl("~/Scripts/jquery/jquery.dropshadow.js")); 

            if(MultiSelect)
            {
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "Dimensions", ResolveUrl("~/Scripts/jquery/jquery.dimensions.js"));
                Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MultiSelect", ResolveUrl("~/Scripts/jquery/jquery.multiselect.js")); 
            }

			if (MaskedInput)
			{
				Page.ClientScript.RegisterClientScriptInclude(typeof(JQuery), "MaskedInput", ResolveUrl("~/Scripts/jquery/jquery.maskedinput-1.2.1.js"));
			}
		}
    }
}