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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolCodeEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolCodeEditorComponentViewExtensionPoint))]
    public class ProtocolCodeEditorComponent : ApplicationComponent
    {
    	private readonly EntityRef _protocolCodeRef;
    	private readonly bool _isNew;
    	private ProtocolCodeDetail _detail;
		private ProtocolCodeSummary _protocolCode;

		/// <summary>
		/// Constructor for adding new protocol code.
		/// </summary>
		public ProtocolCodeEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructor for editing existing code.
		/// </summary>
		/// <param name="protocolCodeRef"></param>
		public ProtocolCodeEditorComponent(EntityRef protocolCodeRef)
		{
			_protocolCodeRef = protocolCodeRef;
		}

		public ProtocolCodeSummary ProtocolCode
		{
			get { return _protocolCode; }
		}

		public override void Start()
		{
			if(_isNew)
			{
				_detail = new ProtocolCodeDetail();
			}
			else
			{
				Platform.GetService<IProtocolAdminService>(
					delegate(IProtocolAdminService service)
					{
						LoadProtocolCodeForEditResponse response = service.LoadProtocolCodeForEdit(
							new LoadProtocolCodeForEditRequest(_protocolCodeRef));
						_detail = response.ProtocolCode;
					});
			}


			base.Start();
		}

        #region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _detail.Name; }
            set
            {
				_detail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
			get { return _detail.Description; }
            set
            {
				_detail.Description = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveProtocolCode, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                {
					if(_isNew)
					{
						AddProtocolCodeResponse response = service.AddProtocolCode(new AddProtocolCodeRequest(_detail));
						_protocolCode = response.ProtocolCode;
					}
					else
					{
						UpdateProtocolCodeResponse response = service.UpdateProtocolCode(new UpdateProtocolCodeRequest(_detail));
						_protocolCode = response.ProtocolCode;
					}
                });
        }
    }
}
