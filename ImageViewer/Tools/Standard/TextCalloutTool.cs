using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activateTextCallout", "global-menus/MenuTools/MenuStandard/MenuTextCallout", "SelectTextCallout")]
	[CheckedStateObserver("activateTextCallout", "IsTextCalloutModeActive", "ModeOrActiveChanged")]
	[Tooltip("activateTextCallout", "TooltipTextCallout")]
	[IconSet("activateTextCallout", IconScheme.Colour, SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon)]
	[GroupHint("activateTextCallout", "Tools.Image.Standard.TextCallout")]
	//
	[ButtonAction("selectTextCallout", "textcallouttool-dropdown/ToolbarTextCallout", "SelectTextCallout", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("selectTextCallout", "IsTextCalloutModeSelected", "ModeChanged")]
	[Tooltip("selectTextCallout", "TooltipTextCallout")]
	[IconSet("selectTextCallout", IconScheme.Colour, SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon)]
	[GroupHint("selectTextCallout", "Tools.Image.Standard.TextCallout")]
	//
	[MenuAction("activateTextOnly", "global-menus/MenuTools/MenuStandard/MenuTextOnly", "SelectTextOnly")]
	[CheckedStateObserver("activateTextOnly", "IsTextOnlyModeActive", "ModeOrActiveChanged")]
	[Tooltip("activateTextOnly", "TooltipTextOnly")]
	[IconSet("activateTextOnly", IconScheme.Colour, SmallTextOnlyIcon, MediumTextOnlyIcon, LargeTextOnlyIcon)]
	[GroupHint("activateTextOnly", "Tools.Image.Standard.TextCallout")]
	//
	[ButtonAction("selectTextOnly", "textcallouttool-dropdown/ToolbarTextOnly", "SelectTextOnly", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("selectTextOnly", "IsTextOnlyModeSelected", "ModeChanged")]
	[Tooltip("selectTextOnly", "TooltipTextOnly")]
	[IconSet("selectTextOnly", IconScheme.Colour, SmallTextOnlyIcon, MediumTextOnlyIcon, LargeTextOnlyIcon)]
	[GroupHint("selectTextOnly", "Tools.Image.Standard.TextCallout")]
	//
	[DropDownButtonAction("activate", "global-toolbars/ToolbarAnnotation/ToolbarTextCallout", "Select", "DropDownMenuModel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSetObserver("activate", "IconSet", "ModeChanged")]
	[GroupHint("activate", "Tools.Image.Standard.TextCallout")]
	//
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class TextCalloutTool : MouseImageViewerTool
	{
		#region Icon Resource Constants

		public const string SmallTextCalloutIcon = "Icons.TextCalloutToolSmall.png";
		public const string MediumTextCalloutIcon = "Icons.TextCalloutToolMedium.png";
		public const string LargeTextCalloutIcon = "Icons.TextCalloutToolLarge.png";
		public const string SmallTextOnlyIcon = "Icons.TextOverlayToolSmall.png";
		public const string MediumTextOnlyIcon = "Icons.TextOverlayToolMedium.png";
		public const string LargeTextOnlyIcon = "Icons.TextOverlayToolLarge.png";

		#endregion

		private DrawableUndoableCommand _undoableCommand;
		private TextCalloutGraphic _textCalloutGraphic;

		public TextCalloutTool() : base(SR.TooltipTextCallout)
		{
			Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		#region Tool Mode Support

		private delegate CreateGraphicState CreateCreateTextCalloutGraphicStateDelegate(TextCalloutGraphic textCalloutGraphic);

		private delegate TextCalloutGraphic CreateTextCalloutGraphic();

		/// <summary>
		/// Fired when the tool's <see cref="Mode"/> changes.
		/// </summary>
		public event EventHandler ModeChanged;

		/// <summary>
		/// Fired when the tool's <see cref="Mode"/> or <see cref="MouseImageViewerTool.Active"/> properties change.
		/// </summary>
		public event EventHandler ModeOrActiveChanged;

		private ActionModelNode _actionModel;
		private ToolSettings _settings;
		private IconSet _textCalloutIconSet;
		private IconSet _textOnlyIconSet;

		// fields specific to a particular mode
		private TextCalloutMode _mode = TextCalloutMode.TextCallout;
		private IconSet _iconSet = null;
		private CreateCreateTextCalloutGraphicStateDelegate _stateCreatorDelegate;
		private CreateTextCalloutGraphic _graphicCreatorDelegate;
		private string _commandCreationName = "";

		/// <summary>
		/// Performs initialization related to the tool's mode support.
		/// </summary>
		protected virtual void InitializeMode()
		{
			_textCalloutIconSet = new IconSet(IconScheme.Colour, SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon);
			_textOnlyIconSet = new IconSet(IconScheme.Colour, SmallTextOnlyIcon, MediumTextOnlyIcon, LargeTextOnlyIcon);

			_settings = ToolSettings.Default;

			try
			{
				_mode = (TextCalloutMode) Enum.Parse(typeof (TextCalloutMode), _settings.TextCalloutMode, true);
			}
			catch (Exception)
			{
				_mode = TextCalloutMode.TextCallout;
			}

			this.OnModeChanged();
			this.OnModeOrActiveChanged();
		}

		/// <summary>
		/// Gets the drop down action model for the tool.
		/// </summary>
		public ActionModelNode DropDownMenuModel
		{
			get
			{
				if (_actionModel == null)
					_actionModel = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Tools.Standard", "textcallouttool-dropdown", this.Actions);
				return _actionModel;
			}
		}

		/// <summary>
		/// Gets or sets the tool's currently selected mode.
		/// </summary>
		public TextCalloutMode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode != value)
				{
					_mode = value;
					this.OnModeChanged();
					this.OnModeOrActiveChanged();
				}
			}
		}

		/// <summary>
		/// Gets the icon set for the tool in the currently selected mode.
		/// </summary>
		public IconSet IconSet
		{
			get { return _iconSet; }
		}

		/// <summary>
		/// Gets the creation command name for the tool in the currently selected mode.
		/// </summary>
		public string CreationCommandName
		{
			get { return _commandCreationName; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is <see cref="MouseImageViewerTool.Active"/> and is in the <see cref="TextCalloutMode.TextCallout"/> mode.
		/// </summary>
		public bool IsTextCalloutModeActive
		{
			get { return this.IsTextCalloutModeSelected && this.Active; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is <see cref="MouseImageViewerTool.Active"/> and is in the <see cref="TextCalloutMode.TextOnly"/> mode.
		/// </summary>
		public bool IsTextOnlyModeActive
		{
			get { return this.IsTextOnlyModeSelected && this.Active; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextCallout"/> mode.
		/// </summary>
		public bool IsTextCalloutModeSelected
		{
			get { return _mode == TextCalloutMode.TextCallout; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextOnly"/> mode.
		/// </summary>
		public bool IsTextOnlyModeSelected
		{
			get { return _mode == TextCalloutMode.TextOnly; }
		}

		/// <summary>
		/// Switches the tool's mode to <see cref="TextCalloutMode.TextCallout"/> and invokes <see cref="MouseImageViewerTool.Select"/>.
		/// </summary>
		public void SelectTextCallout()
		{
			this.Mode = TextCalloutMode.TextCallout;
			this.Select();
		}

		/// <summary>
		/// Switches the tool's mode to <see cref="TextCalloutMode.TextOnly"/> and invokes <see cref="MouseImageViewerTool.Select"/>.
		/// </summary>
		public void SelectTextOnly()
		{
			this.Mode = TextCalloutMode.TextOnly;
			this.Select();
		}

		/// <summary>
		/// Called when the tool's <see cref="Mode"/> changes.
		/// </summary>
		protected virtual void OnModeChanged()
		{
			switch (_mode)
			{
				case TextCalloutMode.TextCallout:
					this.TooltipPrefix = SR.TooltipTextCallout;
					this._commandCreationName = SR.CommandCreateTextCallout;
					this._iconSet = _textCalloutIconSet;
					this._stateCreatorDelegate = CreateTextCalloutGraphicState.Create;
					this._graphicCreatorDelegate = TextCalloutGraphic.CreateTextCalloutGraphic;
					break;
				case TextCalloutMode.TextOnly:
					this.TooltipPrefix = SR.TooltipTextOnly;
					this._commandCreationName = SR.CommandCreateTextOnly;
					this._iconSet = _textOnlyIconSet;
					this._stateCreatorDelegate = CreateTextOnlyGraphicState.Create;
					this._graphicCreatorDelegate = TextCalloutGraphic.CreateTextOnlyGraphic;
					break;
			}
			_settings.TextCalloutMode = _mode.ToString();
			EventsHelper.Fire(ModeChanged, this, new EventArgs());
		}

		/// <summary>
		/// Called when the tool's <see cref="Mode"/> or <see cref="MouseImageViewerTool.Active"/> properties change.
		/// </summary>
		protected virtual void OnModeOrActiveChanged()
		{
			EventsHelper.Fire(ModeOrActiveChanged, this, new EventArgs());
		}

		#endregion

		public override void Initialize()
		{
			base.Initialize();
			InitializeMode();
		}

		protected override void OnActivationChanged()
		{
			base.OnActivationChanged();
			this.OnModeOrActiveChanged();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_textCalloutGraphic != null)
				return _textCalloutGraphic.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			_textCalloutGraphic = _graphicCreatorDelegate();
			_textCalloutGraphic.State = _stateCreatorDelegate(_textCalloutGraphic);

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(_textCalloutGraphic, provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = this.CreationCommandName;
			_undoableCommand.Execute();

			if (_textCalloutGraphic.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_textCalloutGraphic != null)
				return _textCalloutGraphic.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_textCalloutGraphic == null)
				return false;

			if (_textCalloutGraphic.Stop(mouseInformation))
				return true;

			_textCalloutGraphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_undoableCommand = null;
			_textCalloutGraphic = null;
			return false;
		}

		public override void Cancel()
		{
			if (_textCalloutGraphic == null)
				return;

			_textCalloutGraphic.Cancel();

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_textCalloutGraphic = null;
		}
	}

	/// <summary>
	/// Specifies the creation mode of the <see cref="TextCalloutTool"/>.
	/// </summary>
	public enum TextCalloutMode
	{
		/// <summary>
		/// Specifies that the tool should create a text annotation graphic with an arrow indicating a point of interest.
		/// </summary>
		TextCallout,

		/// <summary>
		/// Specifies that the tool should create a standalone text annotation graphic.
		/// </summary>
		TextOnly
	}
}