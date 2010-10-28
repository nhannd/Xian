#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
	public partial class Patient
	{
		public IList<Study> LoadRelatedStudies()
		{
			using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IStudyEntityBroker broker = read.GetBroker<IStudyEntityBroker>();
				StudySelectCriteria criteria = new StudySelectCriteria();
				criteria.PatientKey.EqualTo(Key);
				return broker.Find(criteria);
			}
		}
	}
}
