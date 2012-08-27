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
	/// Defines a template and corresponding variables for use in generating print outs.
	/// </summary>
	public interface IPrintModel
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
