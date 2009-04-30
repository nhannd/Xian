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

#if UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions.Tests
{
	[TestFixture]
	public class ActionModelStoreTests
	{
		private const string MENUNAME = "imageviewer-contextmenu";

		private string[,] _actionSetup;
		private List<IAction> _allActions;

		public ActionModelStoreTests()
		{
		}

		private void CreateActionSet()
		{
			_allActions = new List<IAction>();

			for (int i = 0; i < _actionSetup.Length / 3; ++i)
			{ 
				string actionId = _actionSetup[i, 0];
				string actionPath = _actionSetup[i, 1];
				string groupHint = _actionSetup[i, 2];

				ClickAction action = new ClickAction(actionId, new ActionPath(actionPath, null), ClickActionFlags.None, null);
				action.GroupHint = new GroupHint(groupHint);
				action.Persistent = true;

				_allActions.Add(action);
			}
		}

		private void CreateDefaultXmlDocument()
		{
			XmlDocument document = new XmlDocument();

			XmlElement actionModels = document.CreateElement("action-models");
			document.AppendChild(actionModels);

			XmlElement actionModel = document.CreateElement("action-model");
			actionModels.AppendChild(actionModel);

			actionModel.SetAttribute("id", String.Format("{0}:{1}", this.GetType().FullName, MENUNAME));
			
			List<IAction> defaultActions = _allActions.FindAll(delegate(IAction action) { return action.ActionID.StartsWith("test") == false; });

			foreach (IAction action in defaultActions)
			{
				XmlElement actionElement = document.CreateElement("action");

				actionElement.SetAttribute("id", action.ActionID);
				actionElement.SetAttribute("path", action.Path.ToString());
				actionElement.SetAttribute("group-hint", action.GroupHint.Hint);

				actionModel.AppendChild(actionElement);
			}

			ActionModelSettings.Default.ActionModelsXml = document;
		}

		private void VerifyXmlDocument()
		{
			XmlDocument document = ActionModelSettings.Default.ActionModelsXml;
			XmlElement actionModel = (XmlElement)document.SelectSingleNode("//action-model");

			int i = 0;
			foreach (XmlElement xmlAction in actionModel.ChildNodes)
			{
				string id = xmlAction.GetAttribute("id");
				Assert.AreEqual(id, _allActions[i++].ActionID);
			}
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[TestFixtureTearDown]
		public void CleanUp()
		{ 
		}

		[Test]
		public void TestGroupHintAlgorithm()
		{
			//null
			GroupHint left = new GroupHint(null);
			Assert.AreEqual(left.Hint, "");

			// left = "", right=""
			GroupHint right = new GroupHint("");

			Assert.AreEqual(left.MatchScore(right), 1);

			// left = "NonEmpty.Test", right=""
			left = new GroupHint("NonEmpty.Test");
			Assert.AreEqual(left.MatchScore(right), 1);

			// left = "", right="NonEmpty.Test"
			left = right;
			right = new GroupHint("NonEmpty.Test");
			Assert.AreEqual(left.MatchScore(right), 0);

			// left = "Tools.Standard.ImageManipulation.Lut.LutPresets", right="Tools.Standard.ImageManipulation.Lut"
			left = new GroupHint("Tools.Standard.ImageManipulation.Lut.LutPresets");
			right = new GroupHint("Tools.Standard.ImageManipulation.Lut");
			Assert.AreEqual(left.MatchScore(right), 5);

			// left = "Tools.Standard.ImageManipulation.Lut", right="Tools.Standard.ImageManipulation.Lut.LutPresets"
			GroupHint temp = left;
			left = right;
			right = temp;
			Assert.AreEqual(left.MatchScore(right), 5);

			// left = "Tools.Standard.ImageManipulation.Lut", right="Tools.Standard.ImageManipulation.Lut"
			right = left;
			Assert.AreEqual(left.MatchScore(right), 5);

			// left = "Tools.Standard.ImageManipulation.Lut.LutPresets", right="Tools.Standard.ImageManipulation.Lut.Auto"
			left = new GroupHint("Tools.Standard.ImageManipulation.Lut.LutPresets");
			right = new GroupHint("Tools.Standard.ImageManipulation.Lut.Auto");
			Assert.AreEqual(left.MatchScore(right), 5);

			// left = "Tools.Standard.ImageManipulation.Lut.Auto", right="Tools.Standard.ImageManipulation.Lut.LutPresets"
			left = right;
			right = temp;
			Assert.AreEqual(left.MatchScore(right), 5);
			
			// left = "Tools.Standard.ImageManipulation.Lut", right="DisplaySets"
			right = new GroupHint("DisplaySets");
			Assert.AreEqual(left.MatchScore(right), 0);

			// left = "DisplaySets", right="Tools.Standard.ImageManipulation.Lut"
			temp = left;
			left = right;
			right = temp;
			Assert.AreEqual(left.MatchScore(right), 0);
		}

		[Test]
		public void TestActionModelStore()
		{
			_actionSetup = new string[,]
			{
				{ "ClearCanvas.ImageViewer.Tools.Standard.StackTool:activate", "imageviewer-contextmenu/MenuToolsStandardStack", "Tools.Image.Manipulation.Stacking.Standard" }, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.WindowLevelTool:activate", "imageviewer-contextmenu/MenuToolsStandardWindowLevel", "Tools.Image.Manipulation.Lut.WindowLevel"}, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.LutPresetTool:auto", "imageviewer-contextmenu/MenuToolsStandardLutPresets/auto", "Tools.Image.Manipulation.Lut.Presets" }, 
				{ "BogusDefault", "imageviewer-contextmenu/BogusDefault", "" }, 
				{ "ClearCanvas.ImageViewer.Tools.Standard.PanTool:activate", "imageviewer-contextmenu/MenuToolsStandardPan", "Tools.Image.Manipulation.Pan" },
				{ "ClearCanvas.ImageViewer.Tools.Standard.ZoomTool:activate", "imageviewer-contextmenu/MenuToolsStandardZoom", "Tools.Image.Manipulation.Zoom" } ,
				{ "test1", "imageviewer-contextmenu/Tools.Image.Manipulation.SomethingNew", "Tools.Image.Manipulation.SomethingNew" },
				{ "ClearCanvas.ImageViewer.Tools.Standard.ProbeTool:activate", "imageviewer-contextmenu/MenuToolsStandardProbe", "Tools.Image.Interrogation.Probe" },
				{ "test2", "imageviewer-contextmenu/Tools.Image.SomethingNew", "Tools.Image.SomethingNew" },
				{ "ClearCanvas.ImageViewer.Tools.Volume.ZoomVolumeTool:activate", "imageviewer-contextmenu/Zoom Volume", "Tools.VolumeImage.Manipulation.Zoom" },
				{ "ClearCanvas.ImageViewer.Tools.Volume.RotateVolumeTool:activate", "imageviewer-contextmenu/Rotate Volume", "Tools.VolumeImage.Manipulation.Rotate"},
				{ "ClearCanvas.ImageViewer.Tools.Measurement.RulerTool:activate", "imageviewer-contextmenu/ToolsMeasurementRuler", "Tools.Measurement.Basic.ROI.Linear" },
				{ "ClearCanvas.ImageViewer.DefaultInsertionPoint", "imageviewer-contextmenu/DefaultInsertionPoint", "" },
				{ "test3", "imageviewer-contextmenu/DefaultInsertionPointTest", "" },
				{ "test4", "imageviewer-contextmenu/DefaultInsertionPointTest2", "Something.Completely.New" },
				{ "ClearCanvas.ImageViewer.Layout.Basic.ContextMenuLayoutTool:display0", "imageviewer-contextmenu/DisplaySets", "DisplaySets"},
				{ "test5", "imageviewer-contextmenu/DisplaySets.DisplaySet1", "DisplaySets" },
				{ "test6", "imageviewer-contextmenu/DisplaySets.DisplaySet2", "DisplaySets" },
				{ "test7", "imageviewer-contextmenu/DisplaySets.DisplaySet3", "DisplaySets.DisplaySet3" }
			};

			CreateActionSet();

			CreateDefaultXmlDocument();

			ActionSet allActions = new ActionSet(_allActions);

			ActionModelSettings.Default.BuildAndSynchronize(this.GetType().FullName, MENUNAME, allActions);

			VerifyXmlDocument();
		}
	}
}

#endif