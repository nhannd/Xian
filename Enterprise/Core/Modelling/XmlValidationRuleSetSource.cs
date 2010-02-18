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

using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Abstract base class for implementations of <see cref="IValidationRuleSetSource"/> based on XML validation documents.
	/// </summary>
	public abstract class XmlValidationRuleSetSource : IValidationRuleSetSource
	{
		const string ScriptLanguage = "jscript";
		const string TagValidation = "validation";
		const string TagValidationRule = "validation-rule";
		const string TagApplicabililtyRule = "applicability-rule";
		const string TagValidationRuleset = "validation-ruleset";
		const string AttrClass = "class";

		#region Implementation of IXmlValidationRuleSetSource

		public ValidationRuleSet GetRuleSet(string @class)
		{
			var ruleSetNodes = FindRulesetNodes(@class);

			// return a single rule set that combines all rule sets
			return ValidationRuleSet.Add(CollectionUtils.Map<XmlElement, ValidationRuleSet>(ruleSetNodes, CompileRuleset));
		}

		#endregion

		protected abstract XmlDocument RuleSetDocument { get; }

		private static ValidationRuleSet CompileRuleset(XmlElement rulesetNode)
		{
			var compiler = new XmlSpecificationCompiler(ScriptLanguage);
			var rules = CollectionUtils.Map<XmlElement, ISpecification>(
				rulesetNode.GetElementsByTagName(TagValidationRule), compiler.Compile);

			var applicabilityRuleNode = CollectionUtils.FirstElement(rulesetNode.GetElementsByTagName(TagApplicabililtyRule));
			return applicabilityRuleNode != null ?
				new ValidationRuleSet(rules, compiler.Compile((XmlElement)applicabilityRuleNode))
				: new ValidationRuleSet(rules);
		}

		private XmlNodeList FindRulesetNodes(string @class)
		{
			return RuleSetDocument.SelectNodes(
				string.Format("/{0}/{1}[@{2}='{3}']",
					TagValidation,
					TagValidationRuleset,
					AttrClass,
					@class));
		}
	}
}
