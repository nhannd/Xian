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

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	//[MenuAction("auto", LutPresetTool._globalMenuRoot + "Auto", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("auto", LutPresetTool._viewerKeyboardRoot + "Auto", KeyStroke = XKeys.F2)]
	[MenuAction("auto", LutPresetTool._viewerContextMenuRoot + "Auto", Flags = ClickActionFlags.CheckAction)]
	[ClickHandler("auto", "AutoApplyLut")]
	[EnabledStateObserver("auto", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class LutPresetTool : Tool<IImageViewerToolContext>
	{
		public const string _globalMenuRoot = "global-menus/MenuTools/Standard/MenuToolsStandardLutPresets/";
		public const string _viewerKeyboardRoot = "imageviewer-keyboard/ToolsStandardLutPresets/";
		public const string _viewerContextMenuRoot = "imageviewer-contextmenu/MenuToolsStandardLutPresets/";

		private LutPresetSettings _settings;
		private IActionSet _actionSet;

		private bool _enabled;
		private event EventHandler _enabledChanged;
		
		public LutPresetTool()
		{
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				EventsHelper.Fire(_enabledChanged, this, new EventArgs());
			}
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
								this.Context.Viewer.SelectedPresentationImage is DicomPresentationImage);

			this.Context.Viewer.EventBroker.PresentationImageSelected += new EventHandler<PresentationImageSelectedEventArgs>(OnPresentationImageSelected);
		}

		void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			this.Enabled = e.SelectedPresentationImage is DicomPresentationImage;
		}
		
		private void AutoApplyLut()
		{
			if (!this.Enabled)
				return;

			if (this.Context.Viewer.SelectedPresentationImage == null)
				return;

			if (!(this.Context.Viewer.SelectedPresentationImage is DicomPresentationImage))
				return;

			DicomPresentationImage dicomImage = this.Context.Viewer.SelectedPresentationImage as DicomPresentationImage;

			Window[] windowCenterAndWidth = dicomImage.ImageSop.WindowCenterAndWidth;
			if (windowCenterAndWidth == null || windowCenterAndWidth.Length == 0)
				return;

			WindowLevelApplicator applicator = new WindowLevelApplicator(dicomImage);
			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandWindowLevel;
			command.BeginState = applicator.CreateMemento();

			WindowLevelOperator.InstallVOILUTLinear(dicomImage, windowCenterAndWidth[0].Width, windowCenterAndWidth[0].Center);
			dicomImage.Draw();

			command.EndState = applicator.CreateMemento();
			if (!command.EndState.Equals(command.BeginState))
			{
				applicator.SetMemento(command.EndState);
				dicomImage.ImageViewer.CommandHistory.AddCommand(command);
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
				//generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, _globalMenuRoot, this.GetType().FullName).Action);
				generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, _viewerContextMenuRoot, this.GetType().FullName).Action);
				generatedActions.Add(new LutPresetToolGeneratedAction(this.Context.Viewer, group, _viewerKeyboardRoot, this.GetType().FullName).Action);
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
