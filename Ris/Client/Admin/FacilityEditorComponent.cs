using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;

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
        private Facility _facility;
        private EntityRef<Facility> _facilityRef;
        private IFacilityAdminService _facilityAdminService;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public FacilityEditorComponent()
        {
            _isNew = true;
        }

        public FacilityEditorComponent(EntityRef<Facility> facilityRef)
        {
            _isNew = false;
            _facilityRef = facilityRef;
        }

        public override void Start()
        {
            _facilityAdminService = ApplicationContext.GetService<IFacilityAdminService>();

            if (_isNew)
            {
                _facility = new Facility();
            }
            else
            {
                try
                {
                    _facility = _facilityAdminService.LoadFacility(_facilityRef);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
            
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public Facility Facility
        {
            get { return _facility; }
            set { _facility = value; }
        }

        public string Name
        {
            get { return _facility.Name; }
            set
            {
                _facility.Name = value;
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
                catch (ConcurrencyException e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionConcurrencyFacilityNotSaved, this.Host.DesktopWindow);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private void SaveChanges()
        {
            if (_isNew)
            {
                _facilityAdminService.AddFacility(_facility);
                _facilityRef = new EntityRef<Facility>(_facility);
            }
            else
            {
                _facilityAdminService.UpdateFacility(_facility);
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

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
