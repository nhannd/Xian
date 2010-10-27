#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.PatientReconciliation;

namespace ClearCanvas.Healthcare.Alerts
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    public class ReconciliationAlert : PatientProfileAlertBase
    {
		public override string Id
		{
			get { return "ReconciliationAlert"; }
		}
		
		public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();

            IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, context);
            if (matches.Count > 0)
            {
                return new AlertNotification(this.Id, new string[]{});
            }

            return null;
        }
    }
}
