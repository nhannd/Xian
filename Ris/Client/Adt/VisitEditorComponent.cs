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
    public class VisitEditorComponent : NavigatorComponentContainer
    {
        private EntityRef<Patient> _patientRef;
        private Patient _patient;
        private EntityRef<Visit> _visitRef;
        private Visit _visit;

        private IAdtService _adtService;
        private IAdtReferenceDataService _adtReferenceDataService;

        private VisitDetailsEditorComponent _visitEditor;
        private VisitPractitionersSummaryComponent _visitPractionersSummary;
        private VisitLocationsSummaryComponent _visitLocationsSummary;

        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponent(EntityRef<Patient> patientRef, EntityRef<Visit> visitRef)
        {
            _isNew = false;
            _patientRef = patientRef;
            _visitRef = visitRef;

            //#region testing

            //if (_facilities.Count > 0)
            //{
            //    _visit.Facility = _facilities[0];
            //}
            //else
            //{
            //    Facility facility = new Facility();
            //    facility.Name = "TGH";
            //    _adtReferenceDataService.AddFacility(facility);
            //    _facilities = _adtReferenceDataService.GetFacilities();
            //    _visit.Facility = _facilities[0];
            //}

            //_visit.VisitNumber.Id = "12345";
            //_visit.VisitNumber.AssigningAuthority = "UHN";
            ////_visit.Practitioners.Add(VisitPractitioner.New());
            ////_visit.Locations.Add(VisitLocation.New());


            //#endregion

        }

        public VisitEditorComponent(EntityRef<Patient> patientRef)
        {
            _isNew = true;
            _patientRef = patientRef;
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _adtReferenceDataService = ApplicationContext.GetService<IAdtReferenceDataService>();

            if (_isNew)
            {
                _visit = new Visit();
            }
            else
            {
                _visit = _adtService.LoadVisit(_visitRef, true);
                //this.Host.SetTitle(
                //    string.Format(SR.PatientComponentTitle, _profile.Name.Format(), _profile.Mrn.Format()));
            }


            this.Pages.Add(new NavigatorPage("Visit", _visitEditor = new VisitDetailsEditorComponent()));
            this.Pages.Add(new NavigatorPage("Visit/Practitioners", _visitPractionersSummary = new VisitPractitionersSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Visit/Location", _visitLocationsSummary = new VisitLocationsSummaryComponent()));

            _visitEditor.Visit = _visit;
            //_visitPractionersSummary.Visit = _visit;
            //_visitLocationsSummary.Visit = _visit;

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public override void Accept()
        {
            try
            {
                SaveChanges();
                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (ConcurrencyException)
            {
                this.Host.ShowMessageBox("The visit was modified by another user.  Your changes could not be saved.", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            catch (Exception e)
            {
                Platform.Log(e);
                this.Host.ShowMessageBox("An error occured while attempting to save changes to this item", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            this.Host.Exit();
        }

        public override void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private void SaveChanges()
        {
            if (_isNew)
            {
                _adtService.UpdateVisit(_visit);
                _visitRef = new EntityRef<Visit>(_visit);
            }
            else
            {
                _adtService.UpdateVisit(_visit);
            }
        }
    }
}
