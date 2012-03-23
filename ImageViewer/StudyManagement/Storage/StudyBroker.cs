#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public class StudyBroker : Broker
	{
		internal StudyBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

		public void AddStudy(Study study)
		{
			this.Context.Studies.InsertOnSubmit(study);
		}

		public Study GetStudy(int oid)
		{
			return this.Context.Studies.First(s => s.Oid == oid);
		}

		public Study GetStudy(string studyInstanceUid)
		{
			return this.Context.Studies.First(s => s.StudyInstanceUid == studyInstanceUid);
		}
	}
}
