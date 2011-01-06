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
using ClearCanvas.Common;
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
	/// Manages validation rules sets for domain object classes.
	/// </summary>
	/// <remarks>
	/// All methods on this class are safe for concurrent access by multiple threads.
	/// </remarks>
	internal static class ValidationRuleSetCache
	{
		private const string CacheId = "ClearCanvas.Enterprise.Core.Modelling.EntityValidationRuleSetCache";
		private const string CacheRegion = "default";

		/// <summary>
		/// Invariant rulesets are cached in a static variable, since they cannot change during the lifetime of the process.
		/// </summary>
		private static readonly Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

		/// <summary>
		/// Gets the invariant rule-set (rules that are hard-coded into the domain model).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		internal static ValidationRuleSet GetInvariantRules(Type domainClass)
		{
			lock (_invariantRuleSets)
			{
				ValidationRuleSet rules;

				// return cached rules if possible
				if (_invariantRuleSets.TryGetValue(domainClass, out rules))
					return rules;

				// build rules for domainClass
				var builder = new ValidationBuilder();
				rules = builder.BuildRuleSet(domainClass);

				// cache for future use
				_invariantRuleSets.Add(domainClass, rules);
				return rules;
			}
		}

		/// <summary>
		/// Gets the custom rule-set (rules that are specified in an XML format).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
		internal static ValidationRuleSet GetCustomRules(Type domainClass)
		{
			// Because custom rules may potentially be modified by an administrator, they cannot
			// be cached in a static variable like the invariant rules.
			// instead they are cached using the Cache object with an absolute expiry time, which should be relatively short (a few minutes)
			// This ensures that changes made to these rules will be applied eventually, when the cache expires.

			// however, if the Cache object is not available in this environment, then we have no choice but to build from source
			if(!Cache.IsSupported())
			{
				Platform.Log(LogLevel.Warn, "Caching of custom rules is not supported in this configuration - rules are being compiled from source.");
				return BuildCustomRules(domainClass);
			}

			using (var cacheClient = Cache.CreateClient(CacheId))
			{
				// check the cache for a compiled ruleset
				var ruleSet = (ValidationRuleSet)cacheClient.Get(domainClass.FullName, new CacheGetOptions(CacheRegion));
				if (ruleSet != null)
					return ruleSet;

				// no cached, so compile the ruleset from source
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

		/// <summary>
		/// Builds the custom rule-set (rules that are specified in an XML format).
		/// </summary>
		/// <param name="domainClass"></param>
		/// <returns></returns>
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
