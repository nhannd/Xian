#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Enterprise.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="PasswordConfirmComponent"/>.
    /// </summary>
    [ExtensionPoint]
    public sealed class PasswordConfirmComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PasswordConfirmComponent class.
    /// </summary>
    [AssociateView(typeof(PasswordConfirmComponentViewExtensionPoint))]
    public class PasswordConfirmComponent : ApplicationComponent
    {
        private string _description;
        private string _password;

        #region Constructor

        public PasswordConfirmComponent()
        {
            Description = SR.DescriptionDataAccessGroupChange;
        }

        #endregion

        #region Presentation Model

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                Modified = true;
            }
        }

        [ValidateNotNull]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                Modified = true;
            }
        }


        public void Accept()
        {
            if (HasValidationErrors)
            {
                ShowValidation(true);
                return;
            }
            Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }
  
        #endregion
    }
}
