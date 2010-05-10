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

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	public interface INodePropertiesValidationRule
	{
		bool Validate(AbstractActionModelTreeNode node, string propertyName, object value);
	}

	public sealed class NodePropertiesValidationPolicy : INodePropertiesValidationRule
	{
		private readonly List<INodePropertiesValidationRule> _rules;

		public NodePropertiesValidationPolicy()
		{
			_rules = new List<INodePropertiesValidationRule>();
		}

		public NodePropertiesValidationPolicy(IEnumerable<INodePropertiesValidationRule> rules)
		{
			_rules = new List<INodePropertiesValidationRule>(rules);
		}

		public IList<INodePropertiesValidationRule> Rules
		{
			get { return _rules; }
		}

		public void AddRule(INodePropertiesValidationRule rule)
		{
			Platform.CheckForNullReference(rule, "rule");
			_rules.Add(rule);
		}

		public void AddRule<T>(string propertyName, NodePropertiesValidationRuleDelegate<T> rule) where T : AbstractActionModelTreeNode
		{
			Platform.CheckForEmptyString(propertyName, "propertyName");
			Platform.CheckForNullReference(rule, "rule");
			this.AddRule(new NodePropertiesValidationRule<T>(propertyName, rule));
		}

		public void AddRule<TNode, TValue>(string propertyName, NodePropertiesValidationRuleDelegate<TNode, TValue> rule) where TNode : AbstractActionModelTreeNode
		{
			Platform.CheckForEmptyString(propertyName, "propertyName");
			Platform.CheckForNullReference(rule, "rule");
			this.AddRule(new NodePropertiesValidationRule<TNode, TValue>(propertyName, rule));
		}

		public bool Validate(AbstractActionModelTreeNode node, string propertyName, object value)
		{
			foreach (INodePropertiesValidationRule rule in _rules)
			{
				// if the node fails any single validation rule, it fails all
				if (!rule.Validate(node, propertyName, value))
					return false;
			}
			return true;
		}

		public delegate bool NodePropertiesValidationRuleDelegate<T>(T node, object value) where T : AbstractActionModelTreeNode;

		public delegate bool NodePropertiesValidationRuleDelegate<TNode, TValue>(TNode node, TValue value) where TNode : AbstractActionModelTreeNode;

		private class NodePropertiesValidationRule<T> : INodePropertiesValidationRule where T : AbstractActionModelTreeNode
		{
			private readonly NodePropertiesValidationRuleDelegate<T> _delegate;
			private readonly string _propertyName;

			public NodePropertiesValidationRule(string propertyName, NodePropertiesValidationRuleDelegate<T> @delegate)
			{
				_delegate = @delegate;
				_propertyName = propertyName;
			}

			public bool Validate(AbstractActionModelTreeNode node, string propertyName, object value)
			{
				if (_propertyName != propertyName)
					return true;
				if (!(node is T))
					return true;
				return _delegate.Invoke((T) node, value);
			}
		}

		private class NodePropertiesValidationRule<TNode, TValue> : INodePropertiesValidationRule where TNode : AbstractActionModelTreeNode
		{
			private readonly NodePropertiesValidationRuleDelegate<TNode, TValue> _delegate;
			private readonly string _propertyName;

			public NodePropertiesValidationRule(string propertyName, NodePropertiesValidationRuleDelegate<TNode, TValue> @delegate)
			{
				_delegate = @delegate;
				_propertyName = propertyName;
			}

			public bool Validate(AbstractActionModelTreeNode node, string propertyName, object value)
			{
				if (_propertyName != propertyName)
					return true;
				if (!(node is TNode))
					return true;
				if (!(value is TValue))
					return true;
				return _delegate.Invoke((TNode) node, (TValue) value);
			}
		}
	}
}