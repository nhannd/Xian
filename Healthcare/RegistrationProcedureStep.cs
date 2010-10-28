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
using System.Xml;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(ProcedureStepBuilderExtensionPoint))]
	public class RegistrationProcedureStepBuilder : ProcedureStepBuilderBase
	{

		public override Type ProcedureStepClass
		{
			get { return typeof(RegistrationProcedureStep); }
		}

		public override ProcedureStep CreateInstance(XmlElement xmlNode, Procedure procedure)
		{
			return new RegistrationProcedureStep();
		}

		public override void SaveInstance(ProcedureStep prototype, XmlElement xmlNode)
		{
		}
	}

	public class RegistrationProcedureStep : ProcedureStep
	{
		public RegistrationProcedureStep(Procedure procedure)
			: base(procedure)
		{
		}

		/// <summary>
		/// Default no-args constructor required by NHibernate
		/// </summary>
		public RegistrationProcedureStep()
		{
		}

		public override string Name
		{
			get { return "Registration"; }
		}

		public override bool CreateInDowntimeMode
		{
			get { return true; }
		}

		public override bool IsPreStep
		{
			get { return true; }
		}

		public override TimeSpan SchedulingOffset
		{
			get { return TimeSpan.Zero; }
		}

		public override List<Procedure> GetLinkedProcedures()
		{
			return new List<Procedure>();
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new RegistrationProcedureStep(this.Procedure);
		}

		protected override bool IsRelatedStep(ProcedureStep step)
		{
			// registration steps do not have related steps
			return false;
		}
	}
}
