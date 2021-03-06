#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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