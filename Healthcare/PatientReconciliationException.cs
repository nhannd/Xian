#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Healthcare
{
    public class PatientReconciliationException : HealthcareWorkflowException
    {
        public PatientReconciliationException(String message) : base("Cannot reconcile patients: " + message)
        {
        }
    }
}
