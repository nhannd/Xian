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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Defines an extension point for extensions that act as sources of validation rules.
	/// </summary>
	[ExtensionPoint]
	public class EntityValidationRuleSetSourceExtensionPoint : ExtensionPoint<IValidationRuleSetSource>
	{
	}


	/// <summary>
	/// Provides domain object validation functionality.
	/// </summary>
	public static class Validation
	{
		/// <summary>
		/// Invariant rulesets are cached in a static variable, since they cannot change during the lifetime of the process.
		/// </summary>
		private static readonly Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

		private const string CacheId = "ClearCanvas.Enterprise.Core.Modelling.EntityValidationRuleSetCache";
		private const string CacheRegion = "default";


		#region Public API

		/// <summary>
		/// Validates the specified domain object.
		/// </summary>
		/// <param name="obj"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public static void Validate(DomainObject obj)
		{
			// validate all rules
			Validate(obj, rule => true);
		}

		/// <summary>
		/// Validates the specified domain object, ignoring any rules that do not satisfy the filter.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ruleFilter"></param>
		/// <exception cref="EntityValidationException">Validation failed.</exception>
		public static void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
		{
			var entityClass = obj.GetClass();
			var rules = GetInvariantRules(entityClass)
				.Add(GetCustomRules(entityClass));

			var result = rules.Test(obj, ruleFilter);
			if (result.Fail)
			{
				var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
				throw new EntityValidationException(message, result.Reasons);
			}
		}

		#endregion

		/// <summary>
		/// Gets the invariant rule-set (rules that are hard-coded into the domain model).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		private static ValidationRuleSet GetInvariantRules(Type domainClass)
		{
			lock (_invariantRuleSets)
			{
				ValidationRuleSet rules;

				// return cached rules if possible
				if (_invariantRuleSets.TryGetValue(domainClass, out rules))
					return rules;

				// build rules for entityClass, and put in cache
				var builder = new ValidationBuilder();
				rules = builder.BuildRuleSet(domainClass);
				_invariantRuleSets.Add(domainClass, rules);
				return rules;
			}
		}

		/// <summary>
		/// Gets the custom rule-set (rules that are specified in an XML format).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		private static ValidationRuleSet GetCustomRules(Type domainClass)
		{
			// Because custom rules may potentially be modified by an administrator, they cannot
			// be cached in a static variable like the invariant rules.
			// instead they are cached using the Cache object with an absolute expiry time, which should be relatively short (a few minutes)
			// This ensures that changes made to these rules will be applied eventually, when the cache expires.

			using (var cacheClient = Cache.CreateClient(CacheId))
			{
				// check the cache for a compiled ruleset
				var ruleSet = (ValidationRuleSet)cacheClient.Get(domainClass.FullName, new CacheGetOptions(CacheRegion));
				if (ruleSet != null)
					return ruleSet;

				// no cached, so compile the ruleset for the specified domain class
				ruleSet = BuildCustomRules(domainClass);

				// cache the ruleset if desired
				var settings = new EntityValidationSettings();
				var ttl = TimeSpan.FromSeconds(settings.CustomRulesCachingTimeToLiveSeconds);
				if (ttl > TimeSpan.Zero)
				{
					cacheClient.Put(domainClass.FullName, ruleSet, new CachePutOptions(CacheRegion, ttl, false));
				}

				return ruleSet;
			}
		}

		private static ValidationRuleSet BuildCustomRules(Type domainClass)
		{
			try
			{
				var ruleset = new ValidationRuleSet();

				// combine rules from all sources
				var sources = new EntityValidationRuleSetSourceExtensionPoint().CreateExtensions();
				foreach (IValidationRuleSetSource source in sources)
				{
					var r = source.GetRuleSet(domainClass.FullName);
					if (!r.IsEmpty)
						ruleset = ruleset.Add(r);
				}
				return ruleset;
			}
			catch (Exception e)
			{
				// any exceptions here must be logged and ignored, as their is really no good way to handle them
				Platform.Log(LogLevel.Error, e, "Error attempting to compile custom validation rules");
				return new ValidationRuleSet();
			}
		}
	}
}
