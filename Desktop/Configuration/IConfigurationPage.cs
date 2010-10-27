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
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// An interface defining a configuration page, that is hosted
	/// in a container component along with other such pages
	/// (an example would be a typical Tools/Options dialog).
	/// </summary>
	public interface IConfigurationPage
	{
		/// <summary>
		/// Gets the path to this page.
		/// </summary>
		string GetPath();

		/// <summary>
		/// Gets the <see cref="IApplicationComponent"/> that is hosted in this page.
		/// </summary>
		IApplicationComponent GetComponent();

		/// <summary>
		/// Saves any configuration changes that were made.
		/// </summary>
		void SaveConfiguration();
	}
}
