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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Validate if the value in the number input control is in a specified range.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RangeValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the Port number. If the input is not within 0 and 32000
    /// the Port text box will be highlighted.
    /// 
    /// <clearcanvas:RangeValidator 
    ///                                ID="RangeValidator1" runat="server"
    ///                                ControlToValidate="PortTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                MinValue="0"
    ///                                MaxValue="32000"
    ///                                ErrorMessage="The Port number is not valid.">
    /// </clearcanvas:RangeValidator>
    /// 
    /// </example>
    /// 
    public class RangeValidator: BaseValidator
    {
        #region Private Members
        private int _min;
        private int _max;
        #endregion Private Members

        #region Public Properties


        /// <summary>
        /// Sets or gets the minimum acceptable value.
        /// </summary>
        public int MinValue
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Sets or gets the maximum acceptable value.
        /// </summary>
        public int MaxValue
        {
            get { return _max; }
            set { _max = value; }
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            // Register Javascript for client-side validation

             
            string script =
                "<script language='javascript'>" + @"
                    function " + EvalFunctionName + @"()
                    {
                        extenderCtrl =  document.getElementById('" + this.ClientID + @"');                            
                        textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                        helpCtrl = document.getElementById('" + GetControlRenderID(PopupHelpControlID) + @"');
 
                        result = true;
                        if (textbox.value!=null && textbox.value!='')
                        {
                            portValue = parseInt(textbox.value);
                            result = portValue >= " + _min + @" && portValue<= " + _max + @";
                        }   
                        else
                        {
                            result = false;
                        }
                        
                        if (!result)
                        {
                            extenderCtrl.style.visibility='visible';
                            if (helpCtrl!=null){
                                helpCtrl.style.visibility='visible';
                                helpCtrl.alt='" + ErrorMessage + @"';
                            }
                            textbox.style.backgroundColor ='" + InvalidInputBackColor + @"';
                        }
                        else
                        {
                            //if (helpCtrl!=null)
                            //    helpCtrl.style.visibility='hidden';
                            //extenderCtrl.style.visibility='hidden';
                            //textbox.style.backgroundColor='" + ColorTranslator.ToHtml(BackColor) + @"';
                        }
                        //alert('javascript test=' + result);
                        return result;
                    }

                </script>";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script);

            base.OnInit(e);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (RenderUplevel)
            {
                // Add client-side validation function
                writer.AddAttribute("evaluationfunction", EvalFunctionName);
            }
        }

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            bool result = false;
            TextBox input = FindControl(ControlToValidate) as TextBox;
            if (input != null)
                if (String.IsNullOrEmpty(input.Text) == false)
                {
                    Int32 value;
                    result = Int32.TryParse(input.Text, out value);
                }
            return result;
        }

        #endregion Protected Methods
    }
}
