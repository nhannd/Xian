#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
    public partial class ProcedureStepPerformer : ActivityPerformer
    {
        private Staff _staff;

        /// <summary>
        /// Default constructor - required by NHibernate
        /// </summary>
        public ProcedureStepPerformer()
        {

        }

        public ProcedureStepPerformer(Staff staff)
        {
            Platform.CheckForNullReference(staff, "staff");

            _staff = staff;
        }

        public Staff Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }

        public override bool Equals(object obj)
        {
            ProcedureStepPerformer that = obj as ProcedureStepPerformer;
            return that != null && this._staff.Equals(that._staff);
        }

        public override int GetHashCode()
        {
            return _staff.GetHashCode();
        }
    }
}
