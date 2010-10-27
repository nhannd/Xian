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

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// Rather than making each <see cref="IConfigurationPage"/> a separate
	/// extension of <see cref="ConfigurationPageProviderExtensionPoint"/>, we
	/// use this interface so that related pages can be grouped together.
	/// </summary>
	public interface IConfigurationPageProvider
	{
		/// <summary>
		/// Gets all the <see cref="IConfigurationPage"/>s for this provider.
		/// </summary>
		IEnumerable<IConfigurationPage> GetPages();
	}
}
