using System;

using NUnit.Framework;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services.Tests
{
    [TestFixture]
    public class AdtServiceWithDBTest
    {
        private ISessionManager _sessionManager;
        private IPatientAdminService _patientAdminService;
        private IAdtService _adtService;

        public AdtServiceWithDBTest()
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Platform.InstallDir = "C:\\VS Projects\\ClearCanvas\\Xian\\Trunk\\Desktop\\Executable\\bin\\Debug";

            SessionManagerExtensionPoint xp = new SessionManagerExtensionPoint();
            _sessionManager = (ISessionManager)xp.CreateExtension();
            _sessionManager.InitiateSession();

            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _adtService = ApplicationContext.GetService<IAdtService>();
        }

        [SetUp]
        public void PerTestSetup()
        {

            //PatientProfile pat = new TestPatientProfile();
            //_patientAdminService.AddNewPatient(pat);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (_sessionManager != null)
            {
                _sessionManager.TerminateSession();
            }
        }

        [TearDown]
        public void PerTestTearDown()
        {
        }

        [Test]
        public void CanInitialize()
        {
        }
    }
}
