#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// The exception that is thrown when there is an error is encountered while trying to deserialize DICOM graphics.
	/// </summary>
	public sealed class DicomGraphicsDeserializationException : Exception
	{
		internal DicomGraphicsDeserializationException(string message) : base(message) {}
	}
}