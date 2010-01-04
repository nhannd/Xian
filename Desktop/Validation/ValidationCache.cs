#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Caches attribute and XML-based validation rules for application components.
	/// </summary>
	/// <remarks>
	/// All operations on this class are safe for use by multiple threads.
	/// </remarks>
	public class ValidationCache
	{
		private static readonly ValidationCache _instance = new ValidationCache();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static ValidationCache Instance { get { return _instance; } }


		private readonly Dictionary<Type, List<IValidationRule>> _rulesCache = new Dictionary<Type, List<IValidationRule>>();

		/// <summary>
		/// Constructor
		/// </summary>
		private ValidationCache()
		{
		}

		#region Public API

		/// <summary>
		/// Retrieves rules from cache, or builds rules if not cached.
		/// </summary>
		/// <param name="applicationComponentClass"></param>
		/// <returns></returns>
		public IList<IValidationRule> GetRules(Type applicationComponentClass)
		{
			// locking on the applicationComponentClass here prevents multiple instantiations of the same class
			// from building the same rule-set concurrently
			lock (applicationComponentClass)
			{
				List<IValidationRule> rules;

				// try to get it from the cache
				lock (_rulesCache)
				{
					if (_rulesCache.TryGetValue(applicationComponentClass, out rules))
						return rules;
				}

				// build the validation rules (do this outside of the lock, in case it is slow)
				rules = new List<IValidationRule>();
				rules.AddRange(ProcessAttributeRules(applicationComponentClass));
				rules.AddRange(ProcessCustomRules(applicationComponentClass));

				// cache the rules
				lock (_rulesCache)
				{
					_rulesCache[applicationComponentClass] = rules;
				}

				return rules;
			}
		}

		/// <summary>
		/// Invalidates the rules cache for the specified application component class, causing the rules
		/// to be re-compiled the next time <see cref="GetRules"/> is called.
		/// </summary>
		/// <param name="applicationComponentClass"></param>
		public void Invalidate(Type applicationComponentClass)
		{
			lock (_rulesCache)
			{
				if (_rulesCache.ContainsKey(applicationComponentClass))
					_rulesCache.Remove(applicationComponentClass);
			}
		}

		#endregion

		#region Helpers

		private static List<IValidationRule> ProcessCustomRules(Type applicationComponentClass)
		{
			// if not supported, there are no custom rules
			if (!XmlValidationManager.Instance.IsSupported)
				return new List<IValidationRule>();

			var compiler = new XmlValidationCompiler();
			var ruleNodes = XmlValidationManager.Instance.GetRules(applicationComponentClass);
			return CollectionUtils.Map(ruleNodes, (XmlElement node) => compiler.CompileRule(node));
		}

		private static List<IValidationRule> ProcessAttributeRules(Type applicationComponentClass)
		{
			var rules = new List<IValidationRule>();
			foreach (var property in applicationComponentClass.GetProperties())
			{
				foreach (ValidationAttribute a in property.GetCustomAttributes(typeof(ValidationAttribute), true))
				{
					rules.Add(a.CreateRule(property, new ResourceResolver(applicationComponentClass.Assembly)));
				}
			}

			var methods = applicationComponentClass.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var method in methods)
			{
				foreach (ValidationMethodForAttribute attribute in method.GetCustomAttributes(typeof(ValidationMethodForAttribute), true))
				{
					rules.Add(attribute.CreateRule(method));
				}
			}

			return rules;
		}

		#endregion
	}
}
