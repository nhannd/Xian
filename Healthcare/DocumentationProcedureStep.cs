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

namespace ClearCanvas.Healthcare
{
    public partial class DocumentationProcedureStep : ProcedureStep
    {
        public DocumentationProcedureStep(Procedure procedure)
            : base(procedure)
        {
        }

        /// <summary>
        /// Default no-args constructor required by nHibernate
        /// </summary>
        public DocumentationProcedureStep()
        {
        }

        public override string Name
        {
            get { return "Documentation"; }
        }

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return false; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.MaxValue; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			return new List<Procedure>();
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new DocumentationProcedureStep(this.Procedure);
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// documentation steps do not have related steps
			return false;
		}
	}
}
