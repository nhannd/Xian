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
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="FacilityEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FacilityEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// FacilityEditorComponent class
    /// </summary>
    [AssociateView(typeof(FacilityEditorComponentViewExtensionPoint))]
    public class FacilityEditorComponent : ApplicationComponent
    {
        private IList _informationAuthorityChoices; 
        
        private FacilityDetail _facilityDetail;
        private EntityRef _facilityRef;
        private readonly bool _isNew;

        private FacilitySummary _facilitySummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacilityEditorComponent()
        {
            _isNew = true;
        }

        public FacilityEditorComponent(EntityRef facilityRef)
        {
            _isNew = false;
            _facilityRef = facilityRef;
        }

        public override void Start()
        {
            Platform.GetService<IFacilityAdminService>(
                delegate(IFacilityAdminService service)
                {
                    GetFacilityEditFormDataResponse formResponse = service.GetFacilityEditFormData(new GetFacilityEditFormDataRequest());
                    _informationAuthorityChoices = formResponse.InformationAuthorityChoices;

                    if (_isNew)
                    {
                        _facilityDetail = new FacilityDetail();
                    }
                    else
                    {
                        LoadFacilityForEditResponse response = service.LoadFacilityForEdit(new LoadFacilityForEditRequest(_facilityRef));
                        _facilityRef = response.FacilityDetail.FacilityRef;
                        _facilityDetail = response.FacilityDetail;
                    }
                });

            
            base.Start();
        }

        public FacilitySummary FacilitySummary
        {
            get { return _facilitySummary; }
        }

        public FacilityDetail FacilityDetail
        {
            get { return _facilityDetail; }
            set { _facilityDetail = value; }
        }

        #region Presentation Model

        public IList InformationAuthorityChoices
        {
            get { return _informationAuthorityChoices; }    
        }

        [ValidateNotNull]
        public string Name
        {
            get { return _facilityDetail.Name; }
            set
            {
                _facilityDetail.Name = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Code
        {
            get { return _facilityDetail.Code; }
            set
            {
                _facilityDetail.Code = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public EnumValueInfo InformationAuthority
        {
            get { return _facilityDetail.InformationAuthority; }
            set
            {
                _facilityDetail.InformationAuthority = value;
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
                    Platform.GetService<IFacilityAdminService>(
                        delegate(IFacilityAdminService service)
                        {
                            if (_isNew)
                            {
                                AddFacilityResponse response = service.AddFacility(new AddFacilityRequest(_facilityDetail));
                                _facilityRef = response.Facility.FacilityRef;
                                _facilitySummary = response.Facility;
                            }
                            else
                            {
                                UpdateFacilityResponse response = service.UpdateFacility(new UpdateFacilityRequest(_facilityDetail));
                                _facilityRef = response.Facility.FacilityRef;
                                _facilitySummary = response.Facility;
                            }
                        });

                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveFacility, this.Host.DesktopWindow,
                        delegate
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

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
