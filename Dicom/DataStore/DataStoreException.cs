#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom.DataStore
{
	[Serializable]
	public class DataValidationException : DataStoreException
	{
		internal DataValidationException(string message)
			: base(message)
		{
		}

		internal DataValidationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	[Serializable]
	public class DataStoreException : Exception
	{
		internal DataStoreException(string message)
			: base(message)
		{
		}

		internal DataStoreException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
