#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Defines an interface for configuration <see cref="ApplicationComponent"/>s.
	/// </summary>
	public interface IConfigurationApplicationComponent : IApplicationComponent
	{
		/// <summary>
		/// Save any settings modified in the hosted component.
		/// </summary>
		void Save();
	}
}