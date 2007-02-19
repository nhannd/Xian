using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Imaging;
using System.Xml.Serialization;
using System.IO;
using ClearCanvas.ImageViewer.Tools.Standard.LutPresets;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	//[MenuAction("auto", LutPresetTool._globalMenuRoot + "Auto", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("auto", LutPresetTool.ViewerKeyboardRoot + "Auto", KeyStroke = XKeys.F2)]
	[MenuAction("auto", LutPresetTool.ViewerContextMenuRoot + "Auto", Flags = ClickActionFlags.CheckAction)]
	[ClickHandler("auto", "AutoApplyLut")]
	[EnabledStateObserver("auto", "Enabled", "EnabledChanged")]
	[GroupHint("auto", LutPresetTool.GroupHint)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class LutPresetTool : ImageViewerTool
	{
		public const string GroupHint = "Tools.Image.Manipulation.Lut.Presets";

		public const string GlobalMenuRoot = "global-menus/MenuTools/Standard/MenuToolsStandardLutPresets/";
		public const string ViewerKeyboardRoot = "imageviewer-keyboard/ToolsStandardLutPresets/";
		public const string ViewerContextMenuRoot = "imageviewer-contextmenu/MenuToolsStandardLutPresets/";

		private LutPresetSettings _settings;
		private IActionSet _actionSet;

		public LutPresetTool()
		{
		}

		public override IActionSet Actions
		{
			get
			{
				if (_actionSet == null)
				{
					_actionSet = base.Actions;
					_actionSet = _actionSet.Union(new ActionSet(BuildGeneratedActions()));
				}

				return _actionSet;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			
			_settings = new LutPresetSettings();

			this.Enabled = (this.Context.Viewer.SelectedPresentationImage != null &&
								this.Context.Viewer.SelectedPresentationImage is StandardPresentationImage);

			this.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);
		}

		void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			this.Enabled = 
				(this.SelectedVOILUTLinearProvider != null &&
				this.SelectedVOILUTLinearProvider.VoiLutLinear != null);
		}
		
		private void AutoApplyLut()
		{
			if (!this.Enabled)
				return;

			if (this.SelectedPresentationImage == null ||
				this.SelectedImageSopProvider == null ||
				this.SelectedVOILUTLinearProvider == null ||
				this.SelectedVOILUTLinearProvider.VoiLutLinear == null)
				return;

			Window[] windowCenterAndWidth = this.SelectedImageSopProvider.ImageSop.WindowCenterAndWidth;

			if (windowCenterAndWidth == null || windowCenterAndWidth.Length == 0)
				return;

			WindowLevelApplicator applicator = new WindowLevelApplicator(this.SelectedPresentationImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevel;
			command.BeginState = applicator.CreateMemento();

			this.SelectedVOILUTLinearProvider.VoiLutLinear.WindowWidth = windowCenterAndWidth[0].Width;
			this.SelectedVOILUTLinearProvider.VoiLutLinear.WindowCenter = windowCenterAndWidth[0].Center;
			this.SelectedVOILUTLinearProvider.Draw();

			command.EndState = applicator.CreateMemento();
			if (!command.EndState.Equals(command.BeginState))
			{
				applicator.SetMemento(command.EndState);
				this.Context.Viewer.CommandHistory.AddCommand(command);
			}
		}

		private IEnumerable<IAction> BuildGeneratedActions()
		{
			try
			{
				if (_settings.LutPresetGroups == null || _settings.LutPresetGroups.Count == 0)
					InstallDefaults();
			}
			catch (Exception e)
			{
				Platform.Log(e);
				_settings.LutPresetGroups = new List<LutPresetGroup>();
			}

			List<IAction> generatedActions = new List<IAction>();
			foreach (LutPresetGroup group in _settings.LutPresetGroups)
			{
				//generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, _globalMenuRoot, this.GetType().FullName, LutPresetTool.GroupHint).Action);
				generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, ViewerContextMenuRoot, this.GetType().FullName, LutPresetTool.GroupHint).Action);
				generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, ViewerKeyboardRoot, this.GetType().FullName, LutPresetTool.GroupHint).Action);
			}

			return generatedActions;
		}

		private void InstallDefaults()
		{
			_settings.LutPresetGroups = new List<LutPresetGroup>();
			XmlSerializer serializer = new XmlSerializer(_settings.LutPresetGroups.GetType());
			using (FileStream stream = new FileStream("DefaultLutPresets.xml", FileMode.Open, FileAccess.Read))
				_settings.LutPresetGroups = (List<LutPresetGroup>)serializer.Deserialize(stream);

			_settings.Save();

			KeyStrokeSettings keyStrokeSettings = new KeyStrokeSettings();

			if (keyStrokeSettings.Settings == null || keyStrokeSettings.Settings.Count == 0)
				keyStrokeSettings.Settings = new List<KeyStrokeSetting>();

			List<KeyStrokeSetting> newKeyStrokes = new List<KeyStrokeSetting>();

			serializer = new XmlSerializer(newKeyStrokes.GetType());
			using (FileStream stream = new FileStream("DefaultLutPresetKeyAssignments.xml", FileMode.Open, FileAccess.Read))
				newKeyStrokes = (List<KeyStrokeSetting>)serializer.Deserialize(stream);

			keyStrokeSettings.Settings.AddRange(newKeyStrokes);
			keyStrokeSettings.Save();
		}
	}
}
