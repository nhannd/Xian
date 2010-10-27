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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ReportingWorklistItemKey
	{
		private readonly EntityRef _procedureStepRef;
		private readonly EntityRef _procedureRef;

		public ReportingWorklistItemKey(EntityRef procedureStepRef, EntityRef procedureRef)
		{
			_procedureStepRef = procedureStepRef;
			_procedureRef = procedureRef;
		}

		public EntityRef ProcedureStepRef
		{
			get { return _procedureStepRef; }
		}

		public EntityRef ProcedureRef
		{
			get { return _procedureRef; }
		}
	}
}
