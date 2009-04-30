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
using System.Globalization;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
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
    public class RangeValidator : BaseValidator
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

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            Decimal value;
            if (Decimal.TryParse(GetControlValidationValue(ControlToValidate), NumberStyles.Number, null, out value))
            {
                return value >= MinValue && value <= MaxValue;
            }

            return false;
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            ScriptTemplate template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.RangeValidator.js");
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@", ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@MIN_VALUE@@", MinValue.ToString());
            template.Replace("@@MAX_VALUE@@", MaxValue.ToString());

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}
