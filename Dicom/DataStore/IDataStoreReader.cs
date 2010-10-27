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
	public interface IDataStoreReader : IDisposable
    {
		//TODO: just make this an int.
		long GetStudyCount();
		IStudy GetStudy(string studyInstanceUid);
        IEnumerable<IStudy> GetStudies();
		IEnumerable<IStudy> GetStudiesByStoreTime(bool descending);
		IEnumerable<DicomAttributeCollection> Query(DicomAttributeCollection queryCriteria);
    }
}
