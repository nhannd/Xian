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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ModalityEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ModalityEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ModalityEditorComponent class
    /// </summary>
    [AssociateView(typeof(ModalityEditorComponentViewExtensionPoint))]
    public class ModalityEditorComponent : ApplicationComponent
    {
        private ModalityDetail _modalityDetail;
        private EntityRef _modalityRef;
        private bool _isNew;

        private ModalitySummary _modalitySummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalityEditorComponent()
        {
            _isNew = true;
        }

        public ModalityEditorComponent(EntityRef modalityRef)
        {
            _isNew = false;
            _modalityRef = modalityRef;
        }

        /// <summary>
        /// Gets the summary object that is returned from the add/edit operation
        /// </summary>
        public ModalitySummary ModalitySummary
        {
            get
            {
                return _modalitySummary;
            }
        }

        public override void Start()
        {
            if (_isNew)
            {
                _modalityDetail = new ModalityDetail();
            }
            else
            {
                Platform.GetService<IModalityAdminService>(
                    delegate(IModalityAdminService service)
                    {
                        LoadModalityForEditResponse response = service.LoadModalityForEdit(new LoadModalityForEditRequest(_modalityRef));
                        _modalityRef = response.ModalityDetail.ModalityRef;
                        _modalityDetail = response.ModalityDetail;
                    });
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ModalityDetail ModalityDetail
        {
            get { return _modalityDetail; }
            set { _modalityDetail = value; }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string ID
        {
            get { return _modalityDetail.Id; }
            set
            {
                _modalityDetail.Id = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Name
        {
            get { return _modalityDetail.Name; }
            set
            {
                _modalityDetail.Name = value;
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
                    ExceptionHandler.Report(e, SR.ExceptionSaveModality, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<IModalityAdminService>(
                delegate(IModalityAdminService service)
                {
                    if (_isNew)
                    {
                        AddModalityResponse response = service.AddModality(new AddModalityRequest(_modalityDetail));
                        _modalityRef = response.Modality.ModalityRef;
                        _modalitySummary = response.Modality;
                    }
                    else
                    {
                        UpdateModalityResponse response = service.UpdateModality(new UpdateModalityRequest(_modalityDetail));
                        _modalityRef = response.Modality.ModalityRef;
                        _modalitySummary = response.Modality;
                    }
                });
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
