#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.DataStore
{
	/// <summary>
	/// For use by the Local Data Store service (or equivalent service) only.
	/// </summary>
	public interface IStudyStorageLocator
	{
		string GetStudyStorageDirectory(string studyInstanceUid);
	}
}
