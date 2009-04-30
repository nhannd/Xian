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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Alerts
{
    /// <summary>
    /// A thread-safe singleton class that help to test a domain entity of alerts.
    /// </summary>
    public sealed class AlertHelper
    {
        private static readonly AlertHelper _instance = new AlertHelper();

        private readonly IList<IPatientAlert> _patientAlertTests;
        private readonly IList<IPatientProfileAlert> _patientProfileAlertTests;
        private readonly IList<IOrderAlert> _orderAlertTests;

        public static AlertHelper Instance
        {
            get { return _instance; }
        }

        private AlertHelper()
        {
            _patientAlertTests = new List<IPatientAlert>();
            PatientAlertExtensionPoint patientEP = new PatientAlertExtensionPoint();
            foreach (object test in patientEP.CreateExtensions())
            {
                _patientAlertTests.Add((IPatientAlert)test);
            }

            _patientProfileAlertTests = new List<IPatientProfileAlert>();
            PatientProfileAlertExtensionPoint ppXP = new PatientProfileAlertExtensionPoint();
            foreach (object test in ppXP.CreateExtensions())
            {
                _patientProfileAlertTests.Add((IPatientProfileAlert)test);
            }

            _orderAlertTests = new List<IOrderAlert>();
            OrderAlertExtensionPoint orderEP = new OrderAlertExtensionPoint();
            foreach (object test in orderEP.CreateExtensions())
            {
                _orderAlertTests.Add((IOrderAlert)test);
            }
        }

        /// <summary>
        /// A thread-safe method for testing alerts
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="context"></param>
        /// <returns>A list of alert notifications on this subject</returns>
        public IList<AlertNotification> Test(Entity subject, IPersistenceContext context)
        {
            List<AlertNotification> alertNotifications = new List<AlertNotification>();

            if (subject.Is<Patient>())
            {
                foreach (IPatientAlert test in _patientAlertTests)
                {
                    AlertNotification result = test.Test(subject.Downcast<Patient>(), context);
                    if (result != null)
                        alertNotifications.Add(result);
                }
            }
            else if (subject.Is<PatientProfile>())
            {
                foreach (IPatientProfileAlert test in _patientProfileAlertTests)
                {
                    AlertNotification result = test.Test(subject.Downcast<PatientProfile>(), context);
                    if (result != null)
                        alertNotifications.Add(result);
                }
            }
            else if (subject.Is<Order>())
            {
                foreach (IOrderAlert test in _orderAlertTests)
                {
                    AlertNotification result = test.Test(subject.Downcast<Order>(), context);
                    if (result != null)
                        alertNotifications.Add(result);
                }
            }

            return alertNotifications;
        }
    }
}
