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

using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Specifications;
using System.Xml;
using ClearCanvas.Desktop.Actions;
using System.Diagnostics;

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
	internal sealed partial class ToolSettings
	{
		private ToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public List<ActionFilter> GetActionFilters()
		{
			XmlDocument document = Default.ActionFilterRulesXml;
			XmlElement actionFiltersNode = document.SelectSingleNode("//action-filters") as XmlElement;
			if (actionFiltersNode == null)
				return new List<ActionFilter>();

			List<ActionFilter> actionFilters = new List<ActionFilter>();
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
