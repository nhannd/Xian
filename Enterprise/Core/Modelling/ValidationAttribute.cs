#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
