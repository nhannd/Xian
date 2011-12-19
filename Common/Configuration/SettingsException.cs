#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Represents an exception related to settings.
	/// </summary>
	public class SettingsException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		public SettingsException(string message) : base(message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public SettingsException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
