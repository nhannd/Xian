using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitEditorComponent class
    /// </summary>
    [AssociateView(typeof(VisitEditorComponentViewExtensionPoint))]
    public class VisitEditorComponent : ApplicationComponent
    {
        private Visit _visit;
        private IAdtService _adtService;
        private IAdtReferenceDataService _adtReferenceDataService;
        private IList<Facility> _facilities;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponent(Visit visit)
        {
            _visit = visit;

            _adtService = ApplicationContext.GetService<IAdtService>();
            _adtReferenceDataService = ApplicationContext.GetService<IAdtReferenceDataService>();

            _facilities = _adtReferenceDataService.GetFacilities();

            #region testing

            if (_facilities.Count > 0)
            {
                _visit.Facility = _facilities[0];
            }
            else
            {
                Facility facility = new Facility();
                facility.Name = "TGH";
                _adtReferenceDataService.AddFacility(facility);
                _facilities = _adtReferenceDataService.GetFacilities();
                _visit.Facility = _facilities[0];
            }

            _visit.VisitNumber.Id = "12345";
            _visit.VisitNumber.AssigningAuthority = "UHN";
            //_visit.Practitioners.Add(VisitPractitioner.New());
            //_visit.Locations.Add(VisitLocation.New());


            #endregion

        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }
    }
}
