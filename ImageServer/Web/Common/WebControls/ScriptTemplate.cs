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
using System.IO;
using System.Reflection;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Provides convenience mean to load a javascript template from embedded resource.
    /// </summary>
    public  class ScriptTemplate
    {
        #region Private Members
        private String _script;

        #endregion Private Members

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ScriptTemplate"/>
        /// </summary>
        /// <param name="validator">The validator to which the validator belongs.</param>
        /// <param name="name">Fully-qualified name of the javascript template (including the namespace)</param>
        /// <remarks>
        /// 
        /// </remarks>
        public ScriptTemplate(BaseValidator validator,  string name):
            this(validator.GetType().Assembly, name)
        {
            Replace("@@CLIENTID@@", validator.ClientID);
            Replace("@@INPUT_NAME@@", validator.InputName);
            Replace("@@INPUT_CLIENTID@@", validator.InputControl.ClientID);
            Replace("@@INPUT_NORMAL_BKCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalColor));
            Replace("@@INPUT_INVALID_BKCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputColor));
            Replace("@@INPUT_NORMAL_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InputNormalBorderColor));
            Replace("@@INPUT_INVALID_BORDERCOLOR@@", ColorTranslator.ToHtml(validator.InvalidInputBorderColor));
            Replace("@@INPUT_NORMAL_CSS@@", validator.InputNormalCSS);
            Replace("@@INPUT_INVALID_CSS@@", validator.InvalidInputCSS);            
            Replace("@@INVALID_INPUT_INDICATOR_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.Container.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabel.ClientID);
            Replace("@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@", validator.InvalidInputIndicator == null ? null : validator.InvalidInputIndicator.TooltipLabelContainer.ClientID);
            Replace("@@ERROR_MESSAGE@@", validator.Text);
            Replace("@@IGNORE_EMPTY_VALUE@@", validator.IgnoreEmptyValue? "true":"false");
            
        }

        public ScriptTemplate(Assembly assembly, string name)
        {
            Stream stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
                throw new Exception(String.Format("Resource not found: {0}", name));
            StreamReader reader = new StreamReader(stream);
            _script = reader.ReadToEnd();
            stream.Close();
            reader.Dispose();

        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets or sets the script
        /// </summary>
        public string Script
        {
            get { return _script; }
        }

        #endregion Public properties

        #region Public Methods

        /// <summary>
        /// Replaces a token in the script with the specified value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Replace(string key, string value)
        {
            _script = _script.Replace(key, value);
        }

        #endregion Public Methods
    }
}
