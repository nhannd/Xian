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

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public static class Validation
    {
        private static readonly Dictionary<Type, ValidationRuleSet> _invariantRuleSets = new Dictionary<Type, ValidationRuleSet>();

        public static ValidationRuleSet GetInvariantRules(Type entityClass)
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

        public static ValidationRuleSet GetInvariantRules(DomainObject obj)
        {
            return GetInvariantRules(obj.GetClass());
        }

        public static void Validate(DomainObject obj, Predicate<ISpecification> ruleFilter)
        {
            var rules = GetInvariantRules(obj);

            var result = rules.Test(obj, ruleFilter);
            if (result.Fail)
            {
                var message = string.Format(SR.ExceptionInvalidEntity, TerminologyTranslator.Translate(obj.GetClass()));
                throw new EntityValidationException(message, result.Reasons);
            }
        }

        public static void Validate(DomainObject obj)
        {
            Validate(obj, null);
        }
    }
}
