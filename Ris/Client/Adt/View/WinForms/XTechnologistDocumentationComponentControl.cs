#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.Adt;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="XTechnologistDocumentationComponent"/>
    /// </summary>
    public partial class XTechnologistDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly XTechnologistDocumentationComponent _component;

        private class CustomValidationRule : IValidationRule
        {
            private readonly XTechnologistDocumentationComponentControl _owner;

            public CustomValidationRule(XTechnologistDocumentationComponentControl owner)
            {
                _owner = owner;
            }

            #region IValidationRule Members

            public string PropertyName
            {
                get { return "DUMMY_PROPERTY"; }
            }

            public ValidationResult GetResult(IApplicationComponent component)
            {
                object result = _owner._browser.Document.InvokeScript("hasValidationErrors");

                // if result == null, the hasValidationErrors method is not implemented by the page
                // in this case, assume there are no errors
                bool hasErrors = (result == null) ? false : (bool)result;
                return new ValidationResult(!hasErrors, "");
            }

            #endregion
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public XTechnologistDocumentationComponentControl(XTechnologistDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            // add a validation rule that checks the browser for validation errors
            _component.Validation.Add(new CustomValidationRule(this));

            _browser.ObjectForScripting = _component.ScriptObject;
            // Manually bind the Url since DataBinding was updating the page inappropriately
            _browser.Url = _component.DocumentationPage;
            _component.DocumentationPageChanged += delegate { _browser.Url = _component.DocumentationPage; };
            _component.DocumentationDataChanged += delegate { _browser.Document.InvokeScript("setData"); };
            _component.BeforeDocumentationSaved += _component_BeforeDocumentationSaved;

            _component.ValidationVisibleChanged += _component_ValidationVisibleChanged;

            _procedureStepsTable.Table = _component.ProcedureSteps;
            _procedureStepsTable.ToolbarModel = _component.DocumentationActionModel;
        }

        void _component_BeforeDocumentationSaved(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
        }

        void _component_ValidationVisibleChanged(object sender, EventArgs e)
        {
            _browser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
