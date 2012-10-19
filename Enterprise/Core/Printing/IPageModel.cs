#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Core.Printing
{
	/// <summary>
	/// Defines a model for a page to be printed.
	/// </summary>
	/// <remarks>
	/// A page model consists of an HTML template, and a dictionary of variables to be passed into the template
	/// to produce the final HTML stream to be printed.
	/// </remarks>
	public interface IPageModel
	{
		/// <summary>
		/// Gets the URL of the template.
		/// </summary>
		Uri TemplateUrl { get; }

		/// <summary>
		/// Gets the set of variables accessible to the template.
		/// </summary>
		Dictionary<string, object> Variables { get; } 
	}
}
