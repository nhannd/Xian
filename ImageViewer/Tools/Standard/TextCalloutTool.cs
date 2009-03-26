using System;
using System.Drawing;
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
	[MenuAction("activateTextArea", "global-menus/MenuTools/MenuStandard/MenuTextArea", "SelectTextArea")]
	[CheckedStateObserver("activateTextArea", "IsTextAreaModeActive", "ModeOrActiveChanged")]
	[Tooltip("activateTextArea", "TooltipTextArea")]
	[IconSet("activateTextArea", IconScheme.Colour, SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon)]
	[GroupHint("activateTextArea", "Tools.Image.Standard.TextCallout")]
	//
	[ButtonAction("selectTextArea", "textcallouttool-dropdown/ToolbarTextArea", "SelectTextArea", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("selectTextArea", "IsTextAreaModeSelected", "ModeChanged")]
	[Tooltip("selectTextArea", "TooltipTextArea")]
	[IconSet("selectTextArea", IconScheme.Colour, SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon)]
	[GroupHint("selectTextArea", "Tools.Image.Standard.TextCallout")]
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
		public const string SmallTextAreaIcon = "Icons.TextAreaToolSmall.png";
		public const string MediumTextAreaIcon = "Icons.TextAreaToolMedium.png";
		public const string LargeTextAreaIcon = "Icons.TextAreaToolLarge.png";

		#endregion

		private DrawableUndoableCommand _undoableCommand;
		private InteractiveGraphicBuilder _graphicBuilder;

		public TextCalloutTool() : base(SR.TooltipTextCallout)
		{
			Behaviour |= MouseButtonHandlerBehaviour.SuppressContextMenu | MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		#region Tool Mode Support

		private delegate InteractiveGraphicBuilder CreateInteractiveGraphicBuilderDelegate(StandardStatefulGraphic graphic);

		private delegate StandardStatefulGraphic CreateGraphicDelegate();

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
		private IconSet _textAreaIconSet;

		// fields specific to a particular mode
		private TextCalloutMode _mode = TextCalloutMode.TextCallout;
		private IconSet _iconSet = null;
		private CreateInteractiveGraphicBuilderDelegate _interactiveGraphicBuilderDelegate;
		private CreateGraphicDelegate _graphicDelegateCreatorDelegate;
		private string _commandCreationName = "";

		/// <summary>
		/// Performs initialization related to the tool's mode support.
		/// </summary>
		protected virtual void InitializeMode()
		{
			_textCalloutIconSet = new IconSet(IconScheme.Colour, SmallTextCalloutIcon, MediumTextCalloutIcon, LargeTextCalloutIcon);
			_textAreaIconSet = new IconSet(IconScheme.Colour, SmallTextAreaIcon, MediumTextAreaIcon, LargeTextAreaIcon);

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
		/// Gets a value indicating if the tool is <see cref="MouseImageViewerTool.Active"/> and is in the <see cref="TextCalloutMode.TextArea"/> mode.
		/// </summary>
		public bool IsTextAreaModeActive
		{
			get { return this.IsTextAreaModeSelected && this.Active; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextCallout"/> mode.
		/// </summary>
		public bool IsTextCalloutModeSelected
		{
			get { return _mode == TextCalloutMode.TextCallout; }
		}

		/// <summary>
		/// Gets a value indicating if the tool is in the <see cref="TextCalloutMode.TextArea"/> mode.
		/// </summary>
		public bool IsTextAreaModeSelected
		{
			get { return _mode == TextCalloutMode.TextArea; }
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
		/// Switches the tool's mode to <see cref="TextCalloutMode.TextArea"/> and invokes <see cref="MouseImageViewerTool.Select"/>.
		/// </summary>
		public void SelectTextArea()
		{
			this.Mode = TextCalloutMode.TextArea;
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
					this._interactiveGraphicBuilderDelegate = CreateInteractiveTextCalloutBuilder;
					this._graphicDelegateCreatorDelegate = CreateTextCalloutGraphic;
					break;
				case TextCalloutMode.TextArea:
					this.TooltipPrefix = SR.TooltipTextArea;
					this._commandCreationName = SR.CommandCreateTextArea;
					this._iconSet = _textAreaIconSet;
					this._interactiveGraphicBuilderDelegate = CreateInteractiveTextAreaBuilder;
					this._graphicDelegateCreatorDelegate = CreateTextAreaGraphic;
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

		public override CursorToken GetCursorToken(Point point)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.GetCursorToken(point);
			return base.GetCursorToken(point);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			if (_graphicBuilder != null)
				return _graphicBuilder.Start(mouseInformation);

			IPresentationImage image = mouseInformation.Tile.PresentationImage;
			IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return false;

			StandardStatefulGraphic graphic = _graphicDelegateCreatorDelegate();
			graphic.State = graphic.CreateSelectedState();

			_graphicBuilder = _interactiveGraphicBuilderDelegate(graphic);
			_graphicBuilder.GraphicComplete += OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled += OnGraphicBuilderCancelled;

			_undoableCommand = new DrawableUndoableCommand(image);
			_undoableCommand.Enqueue(new InsertGraphicUndoableCommand(new BasicGraphicToolsControlGraphic(graphic), provider.OverlayGraphics, provider.OverlayGraphics.Count));
			_undoableCommand.Name = this.CreationCommandName;
			_undoableCommand.Execute();

			if (_graphicBuilder.Start(mouseInformation))
				return true;

			this.Cancel();
			return false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder != null)
				return _graphicBuilder.Track(mouseInformation);

			return false;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (_graphicBuilder == null)
				return false;

			if (_graphicBuilder.Stop(mouseInformation))
				return true;

			_graphicBuilder.Graphic.ImageViewer.CommandHistory.AddCommand(_undoableCommand);
			_graphicBuilder = null;
			_undoableCommand = null;
			return false;
		}

		public override void Cancel()
		{
			if (_graphicBuilder == null)
				return;

			_graphicBuilder.Cancel();
		}

		private void OnGraphicBuilderComplete(object sender, GraphicEventArgs e)
		{
			// Find the edit control graphic for the text graphic and invoke edit mode.
			IGraphic graphic = _graphicBuilder.Graphic;
			while (graphic != null && !(graphic is TextEditControlGraphic) && !(graphic is UserCalloutGraphic))
				graphic = graphic.ParentGraphic;
			if (graphic is TextEditControlGraphic)
				((TextEditControlGraphic)graphic).StartEdit();
			else if (graphic is UserCalloutGraphic)
				((UserCalloutGraphic) graphic).StartEdit();

			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_undoableCommand = null;

			_graphicBuilder = null;
		}

		private void OnGraphicBuilderCancelled(object sender, GraphicEventArgs e)
		{
			_graphicBuilder.GraphicComplete -= OnGraphicBuilderComplete;
			_graphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;

			_undoableCommand.Unexecute();
			_undoableCommand = null;

			_graphicBuilder = null;
		}

		private static StandardStatefulGraphic CreateTextCalloutGraphic()
		{
			UserCalloutGraphic callout = new UserCalloutGraphic();
			callout.LineStyle = LineStyle.Solid;
			callout.ShowArrowhead = true;
			return new StandardStatefulGraphic(new VerticesControlGraphic(callout));
		}

		private static InteractiveGraphicBuilder CreateInteractiveTextCalloutBuilder(StandardStatefulGraphic graphic)
		{
			IDecoratorGraphic d = graphic;
			d = d.DecoratedGraphic as IDecoratorGraphic;
			return new InteractiveTextCalloutBuilder(d.DecoratedGraphic as UserCalloutGraphic);
		}

		private static StandardStatefulGraphic CreateTextAreaGraphic()
		{
			return new StandardStatefulGraphic(new TextEditControlGraphic(new MoveControlGraphic(new InvariantTextPrimitive())));
		}

		private static InteractiveGraphicBuilder CreateInteractiveTextAreaBuilder(StandardStatefulGraphic graphic)
		{
			return new InteractiveTextAreaBuilder(graphic.Subject as ITextGraphic);
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
		TextArea
	}
}