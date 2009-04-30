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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Samples.Google.Calendar.Mail
{
    /// <summary>
    /// Extension point for views onto <see cref="EmailComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EmailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EmailComponent class
    /// </summary>
    [AssociateView(typeof(EmailComponentViewExtensionPoint))]
    public class EmailComponent : ApplicationComponent
    {
        private string _emailAddress;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailComponent()
        {
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// Gets or sets the email address field.
        /// </summary>
        [ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        /// <summary>
        /// Called when user presses OK.
        /// </summary>
        public void Accept()
        {
            // check for validation errors
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            // exit normally
            this.Exit(ApplicationComponentExitCode.Normal);
        }

        /// <summary>
        /// Called when user presses Cancel.
        /// </summary>
        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.Cancelled);
        }
    }
}
