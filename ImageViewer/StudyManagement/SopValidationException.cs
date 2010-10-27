#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// An exception that is thrown when Sop validation fails.
	/// </summary>
	public class SopValidationException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/>.
		/// </summary>
		public SopValidationException() {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message.
		/// </summary>
		/// <param name="message"></param>
		public SopValidationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public SopValidationException(string message, Exception inner) : base(message, inner) { }
	}
}
