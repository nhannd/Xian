#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Applied to an entity class to specify how validation of that entity should be handled.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ValidationAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ValidationAttribute()
		{
			this.EnableValidation = true;
		}

		/// <summary>
		/// Gets or sets a value specifying whether validation of this entity is enabled.
		/// </summary>
		/// <remarks>
		/// If set to false, all validation rules defined for this entity, whether defined in code
		/// or in XML, will be ignored.
		/// </remarks>
		public bool EnableValidation { get; set; }

		/// <summary>
		/// Gets the name of the method that supplies high-level validation rules.
		/// </summary>
		/// <remarks>
		/// The method must be static, and have the signature
		/// <code>
		/// IValidationRuleSet MyMethod()
		/// </code>
		/// The rule-set returned by this method will be combined with any rules declared by attributes
		/// on the class or its properties.
		/// </remarks>
		public string HighLevelRulesProviderMethod { get; set; }
	}
}
