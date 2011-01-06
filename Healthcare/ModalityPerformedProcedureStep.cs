#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Core.Modelling;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare
{
    public class ModalityPerformedProcedureStep : PerformedProcedureStep
    {
		private ISet<ClearCanvas.Healthcare.DicomSeries> _dicomSeries;

    	public ModalityPerformedProcedureStep()
    	{
			_dicomSeries = new HashedSet<ClearCanvas.Healthcare.DicomSeries>();
		}

    	public ModalityPerformedProcedureStep(Staff performingStaff, DateTime? startTime)
			: base(performingStaff, startTime)
    	{
			_dicomSeries = new HashedSet<ClearCanvas.Healthcare.DicomSeries>();
		}

		[PersistentProperty]
		public virtual ISet<ClearCanvas.Healthcare.DicomSeries> DicomSeries
		{
			get { return _dicomSeries; }
		}
	}
}
