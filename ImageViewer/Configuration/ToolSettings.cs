#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Specifications;
using System.Xml;
using ClearCanvas.Desktop.Actions;
using System.Diagnostics;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	public class ActionPropertyInfo
	{
		private readonly IAction _action;

		internal ActionPropertyInfo(IAction action)
		{
			_action = action;
		}

		public string Id
		{
			get { return _action.ActionID; }
		}

		public string Site
		{
			get { return _action.Path.Site; }
		}

		public string Path
		{
			get { return _action.Path.ToString(); }
		}

		public string LocalizedPath
		{
			get { return _action.Path.LocalizedPath; }
		}

		public string GroupHint
		{
			get { return _action.GroupHint != null ? _action.GroupHint.Hint : ""; }
		}
	}

	//TODO (CR Sept 2010): remove this stuff
	[ExtensionOf(typeof(ViewerContextMenuFilterExtensionPoint))]
	internal class ViewerContextMenuFilter : ViewerActionFilter
	{
		private IList<ActionFilter> _actionFilters;

		public ViewerContextMenuFilter()
		{
		}

		public override bool Evaluate(IAction action)
		{
			if (!ToolSettings.Default.ApplyActionFilterRules)
				return true;

			if (_actionFilters == null)
				_actionFilters = ToolSettings.Default.GetActionFilters();

			if (_actionFilters.Count == 0)
				return true;

			return CollectionUtils.TrueForAll(_actionFilters, filter => filter.Evaluate(ImageViewer, action));
		}
	}

	internal interface IActionFilterTest
	{
		bool Evaluate(IImageViewer viewer, IAction action);
	}

	internal class ActionFilter : IActionFilterTest
	{
		private ActionFilter()
		{
			ApplicabilityTests = new List<IActionFilterTest>();
		}

		private IList<IActionFilterTest> ApplicabilityTests { get; set; }
		private IActionFilterTest Test { get; set; }

		public static ActionFilter Create(XmlElement actionFilterNode)
		{
			XmlElement actionApplicabilityTestNode = actionFilterNode.SelectSingleNode("applicability-tests/action-test") as XmlElement;
			if (actionApplicabilityTestNode == null)
				return null;

			XmlElement filterTestNode = actionFilterNode.SelectSingleNode("test") as XmlElement;
			if (filterTestNode == null)
				return null;

			IActionFilterTest test = ActionPropertyTest.Create(actionApplicabilityTestNode);
			if (test == null)
				return null; //invalid

			ActionFilter filter = new ActionFilter();
			filter.ApplicabilityTests.Add(test);

			test = ActionPropertyTest.Create(filterTestNode);
			if (test == null)
				return null; //invalid

			filter.Test = test;
			return filter;
		}

		public bool Evaluate(IImageViewer viewer, IAction action)
		{
			if (ApplicabilityTests.Count == 0)
				return true; //this is not a valid test.

			if (Test == null)
				return true; //this is not a valid test.

			foreach (var applicabilityTest in ApplicabilityTests)
			{
				if (!applicabilityTest.Evaluate(viewer, action))
					return true;
			}

			return Test.Evaluate(viewer, action);
		}
	}

	internal abstract class ActionFilterTest : IActionFilterTest
	{
		protected static readonly XmlSpecificationCompiler SpecificationsCompiler = new XmlSpecificationCompiler("jscript");

		#region IActionFilterTest Members

		public abstract bool Evaluate(IImageViewer viewer, IAction action);

		#endregion
	}

	////TODO: later
	//
	//internal class ImagePropertyInfo
	//{
	//    public string Modality { get; set; }
	//}
	//
	//internal class ImagePropertyTest : ActionFilterTest
	//{}

	internal class ActionPropertyTest : ActionFilterTest
	{
		private readonly ISpecification _specification;

		private ActionPropertyTest(ISpecification specification)
		{
			_specification = specification;
		}

		public override bool Evaluate(IImageViewer viewer, IAction action)
		{
			return _specification.Test(new ActionPropertyInfo(action)).Success;
		}

		public static IActionFilterTest Create(XmlElement actionApplicabilityTestNode)
		{
			return new ActionPropertyTest(SpecificationsCompiler.Compile(actionApplicabilityTestNode));
		}
	}

	[SettingsGroupDescription("Default configuration for tools in the viewer.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal sealed partial class ToolSettings
	{
		private ToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public override void Upgrade()
		{
			//disabled.	
		}

		public List<ActionFilter> GetActionFilters()
		{
			List<ActionFilter> actionFilters = new List<ActionFilter>();
			XmlDocument document = Default.ActionFilterRulesXml;
			if (document == null)
				return actionFilters;

			XmlElement actionFiltersNode = document.SelectSingleNode("//action-filters") as XmlElement;
			if (actionFiltersNode == null)
				return actionFilters;

			foreach (XmlNode actionFilterNode in actionFiltersNode.ChildNodes)
			{
				if (actionFilterNode.NodeType != XmlNodeType.Element)
					continue;

				ActionFilter actionFilter = ActionFilter.Create((XmlElement)actionFilterNode);
				if (actionFilter != null)
					actionFilters.Add(actionFilter);
			}

			return actionFilters;
		}
	}
}
