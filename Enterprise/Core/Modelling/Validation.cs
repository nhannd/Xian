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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class Validation
    {
        private static readonly Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

		private const string CacheId = "ClearCanvas.Enterprise.Core.Modelling.EntityValidationRuleSetCache";
    	private const string CacheRegion = "default";


		#region Public API

		public static void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
        {
			var entityClass = obj.GetClass();
			var rules = GetInvariantRules(entityClass)
				.Combine(GetCustomRules(entityClass));

            var result = rules.Test(obj, ruleFilter);
            if (result.Fail)
            {
                var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
                throw new EntityValidationException(message, result.Reasons);
            }
        }

        public static void Validate(DomainObject obj)
        {
			// validate all rules
            Validate(obj, rule => true);
		}

		#endregion

		private static ValidationRuleSet GetInvariantRules(Type entityClass)
		{
			lock (_invariantRuleSets)
			{
				ValidationRuleSet rules;

				// return cached rules if possible
				if (_invariantRuleSets.TryGetValue(entityClass, out rules))
					return rules;

				// build rules for entityClass, and put in cache
				var builder = new ValidationBuilder();
				rules = builder.BuildRuleSet(entityClass);
				_invariantRuleSets.Add(entityClass, rules);
				return rules;
			}
		}

		private static ValidationRuleSet GetCustomRules(Type entityClass)
		{
			// Because custom rules may potentially be modified by an administrator, they cannot
			// be cached in a static variable like the invariant rules.
			// instead they are cached using the Cache object with an absolute expiry time, which should be relatively short (a few minutes)
			// This ensures that changes made to these rules will be applied eventually, when the cache expires.

			using (var cacheClient = Cache.CreateClient(CacheId))
			{
				var ruleSet = (ValidationRuleSet)cacheClient.Get(entityClass.FullName, new CacheGetOptions(CacheRegion));
				if (ruleSet != null)
					return ruleSet;

				var builder = new XmlValidationBuilder();
				ruleSet = builder.GetRuleset(entityClass);
				cacheClient.Put(entityClass.FullName, ruleSet, new CachePutOptions(CacheRegion, TimeSpan.FromMinutes(2), false));
				return ruleSet;
			}
		}
	}
}
