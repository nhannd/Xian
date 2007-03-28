using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IAlertService))]
    public class AlertService : ApplicationServiceBase, IAlertService
    {
        IList<IAlert> _alertTests;

        public AlertService()
        {
            AlertExtensionPoint xp = new AlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _alertTests = new List<IAlert>();
            foreach (object o in tests)
            {
                _alertTests.Add((IAlert)o);
            }
        }

        [ReadOperation]
        public GetAlertsByPatientProfileResponse GetAlertsByPatientProfile(GetAlertsByPatientProfileRequest request)
        {
            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(request.PatientProfileRef);

            profileBroker.LoadPatientForPatientProfile(profile);
            Patient patient = profile.Patient;

            return new GetAlertsByPatientProfileResponse(GetAlertsHelper(patient));
        }

        [ReadOperation]
        public GetAlertsByPatientResponse GetAlertsByPatient(GetAlertsByPatientRequest request)
        {
            IPatientBroker broker = PersistenceContext.GetBroker<IPatientBroker>();
            Patient patient = broker.Load(request.PatientRef);

            return new GetAlertsByPatientResponse(GetAlertsHelper(patient));
        }

        private List<AlertNotificationDetail> GetAlertsHelper(Patient patient)
        {
            AlertAssembler assembler = new AlertAssembler();
            List<AlertNotificationDetail> results = new List<AlertNotificationDetail>();

            foreach (IAlert alertTest in _alertTests)
            {
                IAlertNotification testResult = alertTest.Test(patient);
                if (testResult != null)
                {
                    results.Add(assembler.CreateAlertNotification(testResult));
                }
            }

            return results;
        }
    }
}
