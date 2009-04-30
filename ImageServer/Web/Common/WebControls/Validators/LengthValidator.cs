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

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validate length of the input.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example adds min password length validation. 
    /// </code>
    /// <uc1:InvalidInputIndicator ID="InvalidZipCodeIndicator" runat="server" 
    ///     ImageUrl="~/images/icons/HelpSmall.png"
    ///     Visible="true"/>
    ///                                                        
    /// <clearcanvas:FilesystemPathValidator runat="server" ID="PasswordValidator" 
    ///         ControlToValidate="PasswordTextBox"
    ///         InputName="Password" 
    ///         InvalidInputColor="#FAFFB5" 
    ///         InvalidInputIndicatorID="InvalidPasswordIndicator"
    ///         MinLength="8"
    ///         ErrorMessage="Invalid password"
    ///         Display="None" ValidationGroup="vg1"/> 
    /// </code>
    /// </example>
    public class LengthValidator : BaseValidator
    {
        #region Private Members

        private int _minLength = Int32.MinValue;
        private int _maxLength = Int32.MaxValue;

        #endregion Private Members

        #region Public Properties

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set { _maxLength = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }


        protected override bool OnServerSideEvaluate()
        {
            //String value = GetControlValidationValue(ControlToValidate);
            //if (value == null || value.Length < MinLength || value.Length>MaxLength)
            //{
            //    ErrorMessage = String.Format("Must be at least {0} and no more than {1} characters.", MinLength, MaxLength);
            //    return false;
            //}
            //else
            //    return true;
            return true;
        }

        #endregion Protected Methods

        protected override void RegisterClientSideValidationExtensionScripts()
        {
            ScriptTemplate template =
                new ScriptTemplate(this, "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.LengthValidator.js");

            template.Replace("@@MIN_LENGTH@@", MinLength.ToString());
            template.Replace("@@MAX_LENGTH@@", MaxLength.ToString());
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@", ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");
            
            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }
    }
}
