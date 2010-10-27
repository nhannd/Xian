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

namespace ClearCanvas.Dicom.DataStore
{
	public interface IDataStoreStudyRemover : IDisposable
	{
		void ClearAllStudies();
		void RemoveStudies(IEnumerable<string> studyInstanceUids);
		void RemoveStudy(string studyInstanceUid);
	}
}
