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
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    [Themeable(true)]
    public partial class InvalidInputIndicator : UserControl, IInvalidInputIndicator
    {
        private int _referenceCounter;

        public String ImageUrl
        {
            get { return Image.ImageUrl; }
            set { Image.ImageUrl = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ContainerPanel.Attributes.Add("shared", _referenceCounter>1? "true":"false");
            ContainerPanel.Attributes.Add("numberofinvalidfields", "0");
 
            ContainerPanel.Style.Add("display", "block");
            ContainerPanel.Style.Add("visibility", "hidden");   

        }
                
           
        public Control Container
        {
            get { return ContainerPanel; }
        }


        public void Show()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "visible";

        }

        public void Hide()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "hidden";
             
        }


        public Label TooltipLabel
        {
            get { return HintLabel; }
        }

        

        public void AttachValidator(Common.WebControls.Validators.BaseValidator validator)
        {
            _referenceCounter ++;
            validator.InputControl.Attributes.Add("multiplevalidators", _referenceCounter > 1 ? "true" : "false");
        }


        #region IInvalidInputIndicator Members


        public Control TooltipLabelContainer
        {
            get
            {
                return HintPanel;

            }
        }

        #endregion
    }
}