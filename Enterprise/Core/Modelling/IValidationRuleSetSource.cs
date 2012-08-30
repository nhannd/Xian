#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an interface to a class that acts as a source of validation rule sets.
	/// </summary>
	public interface IValidationRuleSetSource
	{
		/// <summary>
		/// Gets a value indicating whether the rules provided by this source are static - that is,
		/// they cannot change during the lifetime of the process.
		/// </summary>
		/// <remarks>
		/// If this flag returns true, calls to <see cref="GetRuleSet"/> will cache the return value
		/// for the life of the process, which may provide a slight optimization.  If false, the
		/// return value may still be cached, but only for a relatively short period of time.
		/// </remarks>
		bool IsStatic { get; }

		/// <summary>
		/// Gets the validation rule set for the specified class.
		/// </summary>
		/// <param name="domainClass"> </param>
		/// <returns></returns>
		ValidationRuleSet GetRuleSet(Type domainClass);
	}
}
