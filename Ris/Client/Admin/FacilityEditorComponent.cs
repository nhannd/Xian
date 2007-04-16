using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;

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
        private FacilityDetail _facilityDetail;
        private EntityRef _facilityRef;
        private bool _isNew;

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
            try
            {
                if (_isNew)
                {
                    _facilityDetail = new FacilityDetail();
                }
                else
                {
                    Platform.GetService<IFacilityAdminService>(
                        delegate(IFacilityAdminService service)
                        {
                            LoadFacilityForEditResponse response = service.LoadFacilityForEdit(new LoadFacilityForEditRequest(_facilityRef));
                            _facilityRef = response.FacilityRef;
                            _facilityDetail = response.FacilityDetail;
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
            
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public FacilityDetail FacilityDetail
        {
            get { return _facilityDetail; }
            set { _facilityDetail = value; }
        }

        #region Presentation Model

        public string Name
        {
            get { return _facilityDetail.Name; }
            set
            {
                _facilityDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Code
        {
            get { return _facilityDetail.Code; }
            set
            {
                _facilityDetail.Code = value;
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
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        private void SaveChanges()
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
                        }
                        else
                        {
                            UpdateFacilityResponse response = service.UpdateFacility(new UpdateFacilityRequest(_facilityRef, _facilityDetail));
                            _facilityRef = response.Facility.FacilityRef;
                        }
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
