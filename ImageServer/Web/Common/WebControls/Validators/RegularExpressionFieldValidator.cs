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
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate if an input control value matches a specified regular expression.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RegularExpressionFieldValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the IP Address. If the input is not an IP address, the IP address
    /// text box will be highlighted.
    /// 
    /// <clearcanvas:RegularExpressionFieldValidator 
    ///                                ID="RegularExpressionFieldValidator1" runat="server"
    ///                                ControlToValidate="IPAddressTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
    ///                                ErrorMessage="The IP address is not valid." Display="None">
    /// </clearcanvas:RegularExpressionFieldValidator>
    /// 
    /// </example>
    /// 
    public class RegularExpressionFieldValidator : BaseValidator
    {
        #region Private Members

        private string _regEx;
        private bool _required = false;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets or gets the regular expression to validate the input.
        /// </summary>
        public string ValidationExpression
        {
            get { return _regEx; }
            set { _regEx = value; }
        }


        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            string value = GetControlValidationValue(ControlToValidate);
            Regex regex = new Regex(ValidationExpression);
            if (value != null)
                return regex.IsMatch(value);
            else
                return false;
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            ScriptTemplate template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.RegularExpressionValidator.js");
            template.Replace("@@REQUIRED@@", _required? "true":"false");
            template.Replace("@@REGULAR_EXPRESSION@@", ValidationExpression.Replace("\\", "\\\\").Replace("'", "\\'"));
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@", ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");
            

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}
