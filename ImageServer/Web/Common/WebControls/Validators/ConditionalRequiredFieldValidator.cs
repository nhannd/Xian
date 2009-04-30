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
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
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
    public class ConditionalRequiredFieldValidator : BaseValidator
    {
        #region Private Members


        #endregion Private members

        #region Public Properties

        #endregion Public Properties

        #region Protected Methods

        //protected override void RegisterClientSideValidationExtensionScripts()
        //{
        //    if (ConditionalCheckBoxID != null)
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "ClearCanvas.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate_Conditional.js");
        //        template.Replace("@@CLIENTID@@", ClientID);
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@CONDITIONAL_CONTROL_CLIENTID@@", GetControlRenderID(ConditionalCheckBoxID));
        //        template.Replace("@@REQUIRED_WHEN_CHECKED@@", RequiredWhenChecked.ToString().ToLower());
        //        template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue.ToString().ToLower());
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //    else
        //    {
        //        ScriptTemplate template =
        //            new ScriptTemplate(GetType().Assembly,
        //                               "ClearCanvas.ImageServer.Web.Common.WebControls.ConditionalRequiredFieldValidator_OnValidate.js");
        //        template.Replace("@@CLIENTID@@", ClientID); 
        //        template.Replace("@@FUNCTION_NAME@@", ClientEvalFunctionName);
        //        template.Replace("@@INPUT_CLIENTID@@", InputControl.ClientID);
        //        template.Replace("@@ERROR_MESSAGE@@", ErrorMessage);

        //        Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientEvalFunctionName, template.Script, true);
        //    }
        //}

        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            

            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            base.RegisterClientSideBaseValidationScripts();

            ScriptTemplate template =
                new ScriptTemplate(this,
                                   "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.ConditionalRequiredFieldValidator.js");

            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@", ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");
            
            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}
