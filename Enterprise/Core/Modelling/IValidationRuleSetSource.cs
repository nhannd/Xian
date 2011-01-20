#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an interface to a class that acts as a source of validation rule sets.
	/// </summary>
	public interface IValidationRuleSetSource
	{
		/// <summary>
		/// Gets the validation rule set for the specified class.
		/// </summary>
		/// <param name="class"></param>
		/// <returns></returns>
		ValidationRuleSet GetRuleSet(string @class);
	}
}
