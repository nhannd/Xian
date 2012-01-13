#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// An interface that provides direct access to the configuration settings
	/// without having to instantiate a configuration component.
	/// </summary>
	public interface IConfigurationProvider
	{
		string SettingsClassName { get; }
		void UpdateConfiguration(Dictionary<string, string> settings);
	}
}
