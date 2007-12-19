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
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Validate a required Web UI input control containing value based on the state of another checkbox control.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RequiredFieldValidator"/>.
    /// Users can use this control for required field validation based on state of a checkbox on the UI. When the 
    /// condition is satisfied, the control will validate the input field contains a value. Developers 
    /// can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the SIN if the citizen checkbox is checked:
    /// 
    /// <clearcanvas:ConditionalRequiredFieldValidator 
    ///                                ID="RequiredFieldValidator2" runat="server" 
    ///                                ControlToValidate="SINTextBox"
    ///                                ConditionalCheckBoxID="IsCitizenCheckedBox" 
    ///                                RequiredWhenChecked="true"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                EnableClientScript="true"
    ///                                ErrorMessage="SIN is required for citizen!!">
    /// </clearcanvas:ConditionalRequiredFieldValidator>
    /// 
    /// </example>
    /// 
    public class ConditionalRequiredFieldValidator: BaseValidator
    {
        #region Private Members
        // the control, if checked, "enables" this validation control
        private string _conditionalCheckBoxID;
        // specify when the validation should be enabled: when the condition control is checked or unchecked
        private bool    _requiredWhenChecked;


        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets or gets the ID of the condition control.
        /// </summary>
        /// <remarks>
        /// The condition control indicates whether the input control associated with the validator
        /// control must contain a value.
        /// 
        /// If <seealso cref="ConditionalCheckBoxID"/> is not specified, <seealso cref="ConditionalRequiredFieldValidator"/>
        /// behaves the same as <seealso cref="RequiredFieldValidator"/> (ie, the input field must always contains value).
        /// </remarks>
        public string ConditionalCheckBoxID
        {
            get { return _conditionalCheckBoxID; }
            set { _conditionalCheckBoxID = value; }
        }

        /// <summary>
        /// Indicates whether the input control must contain a value 
        /// when the checkbox specified by <seealso cref="ConditionalCheckBoxID"/>
        /// is checked or is unchecked.
        /// </summary>
        public bool RequiredWhenChecked
        {
            get { return _requiredWhenChecked; }
            set { _requiredWhenChecked = value; }
        }


        #endregion Public Properties


        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            TextBox input = FindControl(ControlToValidate) as TextBox;
            CheckBox ConditionalCheckBox = (String.IsNullOrEmpty(ConditionalCheckBoxID))? null:FindControl(ConditionalCheckBoxID) as CheckBox;

            // Register the javascripts to be used by the validator controls on the client
            string script;

            
            // Create different javascript based depending on whether the condition checkbox is specified
            if (ConditionalCheckBoxID!= null)
            {
                script =
                    "<script language='javascript'>" + @"
                        function " + EvalFunctionName + @"()
                        {
                                result = true;

                                chkbox = document.getElementById('" + ConditionalCheckBox.ClientID + @"');
                                textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                                extenderCtrl =  document.getElementById('" + this.ClientID + @"');
                                helpCtrl = document.getElementById('" + GetControlRenderID(PopupHelpControlID) + @"');

                                
                                if (chkbox.checked == " + _requiredWhenChecked.ToString().ToLower() + @")
                                {
                                    if (textbox.value==null || textbox.value=='')
                                    {
                                        result = false;
                                    }
                                }

                                 if (!result)
                                {
                                    textbox.style.backgroundColor ='" + InvalidInputBackColor + @"';
                                    if (helpCtrl!=null){
                                        helpCtrl.style.visibility='visible';
                                        helpCtrl.alt='" + ErrorMessage + @"';
                                    }
                                }
                                else
                                {
                                    //if (helpCtrl!=null)
                                    //    helpCtrl.style.visibility='hidden';
                                }

                                return  result;
                        }

                        </script>
                    ";
            }
            else
            {
                script =
                    "<script language='javascript'>" + @"
                        function " + EvalFunctionName + @"()
                        {

                                result = true;

                                textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                                extenderCtrl =  document.getElementById('" + this.ClientID + @"');
                                helpCtrl = document.getElementById('" + GetControlRenderID(PopupHelpControlID) + @"');

                                if (textbox.value==null || textbox.value=='')
                                {
                                    result = false;
                                }
                            
                                if (!result)
                                {
                                    if (helpCtrl!=null)
                                    {
                                        helpCtrl.style.visibility='visible';
                                        helpCtrl.alt='" + ErrorMessage + @"';
                                    }
                                    textbox.style.backgroundColor ='" + InvalidInputBackColor + @"';
                                }
                                else
                                {
                                    //if (helpCtrl!=null)
                                    //    helpCtrl.style.visibility='hidden';
                                    //textbox.style.backgroundColor='" + System.Drawing.ColorTranslator.ToHtml(BackColor) + @"';
                                }

                                return  result;
                        }

                        </script>
                    ";
            }

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script);

            base.OnInit(e);
        }


        /// <summary>
        /// Called during server side validation
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            TextBox input = FindControl(ControlToValidate) as TextBox;

            CheckBox chkbox = String.IsNullOrEmpty(ConditionalCheckBoxID)? null:FindControl(ConditionalCheckBoxID) as CheckBox;

            if (chkbox != null)
            {
                if (chkbox.Checked == _requiredWhenChecked)
                {
                    string value = GetControlValidationValue(this.ControlToValidate);
                    if (String.IsNullOrEmpty(value))
                    {
                        input.BackColor = Color.Yellow;
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                string value = GetControlValidationValue(this.ControlToValidate);
                if (String.IsNullOrEmpty(value))
                {
                    input.BackColor = Color.Yellow;
                    return false;
                }
            }
            return true;
        }

        #endregion Protected Methods
    }

    
}
