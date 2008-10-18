#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public partial class WindowLevelTool
	{
		private class PresetVoiLutActionContainer
		{
			private readonly WindowLevelTool _ownerTool;
			private readonly PresetVoiLut _preset;
			private readonly MenuAction _action;

			public PresetVoiLutActionContainer(WindowLevelTool ownerTool, string actionPathPrefix, PresetVoiLut preset, int index)
			{
				_ownerTool = ownerTool;
				_preset = preset;

				string actionId = String.Format("apply{0}", _preset.Operation.Name);
				ActionPath actionPath = new ActionPath(String.Format("{0}/presetLut{1}", actionPathPrefix, index), ownerTool._resolver);
				_action = new MenuAction(actionId, actionPath, ClickActionFlags.None, ownerTool._resolver);
				_action.Persistent = false;
				_action.GroupHint = new GroupHint("Tools.Image.Manipulation.Lut.VoiLuts");
				_action.Label = _preset.Operation.Name;
				_action.KeyStroke = _preset.KeyStroke;
				_action.SetClickHandler(this.Apply);
			}

			public ClickAction Action
			{
				get { return _action; }
			}

			private void Apply()
			{
				ImageOperationApplicator applicator = new ImageOperationApplicator(_ownerTool.SelectedPresentationImage, _preset.Operation);
				MemorableUndoableCommand command = new MemorableUndoableCommand(applicator);
				command.BeginState = applicator.CreateMemento();

				applicator.ApplyToAllImages();

				command.EndState = applicator.CreateMemento();
				if (!command.EndState.Equals(command.BeginState))
					_ownerTool.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private readonly IResourceResolver _resolver = new ResourceResolver(typeof(WindowLevelTool).Assembly);

		public ActionModelNode PresetDropDownMenuModel
		{
			get
			{
				ActionModelRoot root = new ActionModelRoot();
				root.InsertActions(CreateActions("windowlevel-dropdown").ToArray());
				if (root.ChildNodes.Count == 0)
				{
					ClickAction dummy = new ClickAction("dummy",
						new ActionPath("windowlevel-dropdown/dummy", _resolver),
						ClickActionFlags.None, _resolver);
					dummy.Persistent = false;
					dummy.Label = SR.LabelNone;
					root.InsertAction(dummy);
				}
				return root;
			}
		}

		public override IActionSet Actions
		{
			get
			{
				IActionSet baseActions = base.Actions;
				return baseActions.Union(new ActionSet(CreateActions("imageviewer-contextmenu/PresetVoiLuts")));
			}
		}

		private List<IAction> CreateActions(string actionPathPrefix)
		{
			List<IAction> actions = new List<IAction>();

			if (this.SelectedPresentationImage is IImageSopProvider)
			{
				List<PresetVoiLut> presets = new List<PresetVoiLut>();

				//Only temporary until we enable the full functionality in the presets.
				PresetVoiLut autoPreset = new PresetVoiLut(new AutoPresetVoiLutOperationComponent());
				autoPreset.KeyStroke = XKeys.F2;
				presets.Add(autoPreset);

				ImageSop sop = ((IImageSopProvider) this.SelectedPresentationImage).ImageSop;

				PresetVoiLutGroupCollection groups = PresetVoiLutSettings.Default.GetPresetGroups();
				PresetVoiLutGroup group = CollectionUtils.SelectFirst(groups,
															delegate(PresetVoiLutGroup testGroup)
																{
																	return testGroup.AppliesTo(sop);
																});
				if (group != null)
				{
					foreach (PresetVoiLut preset in group.Clone().Presets)
					{
						if (preset.Operation.AppliesTo(this.SelectedPresentationImage))
							presets.Add(preset);
					}
				}

				int i = 0;
				presets.Sort(new PresetVoiLutCollectionSortByKeyStrokeSortByName());

				foreach (PresetVoiLut preset in presets)
					actions.Add(new PresetVoiLutActionContainer(this, actionPathPrefix, preset, ++i).Action);
			}

			return actions;
		}
	}
}
