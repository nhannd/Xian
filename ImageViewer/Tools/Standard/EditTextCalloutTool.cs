using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("edit", "basicgraphic-menu/MenuEdit", "Edit")]
	[IconSet("edit", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
	[VisibleStateObserver("edit", "Visible", "VisibleChanged")]
	[GroupHint("edit", "Tools.Image.Annotation.TextCallout.Edit")]
	[Tooltip("edit", "TooltipEdit")]
	[ExtensionOf(typeof (GraphicToolExtensionPoint))]
	public class EditTextCalloutTool : GraphicTool
	{
		public EditTextCalloutTool() {}

		private UserCalloutGraphic UserCalloutGraphic
		{
			get
			{
				TextCalloutGraphic textCalloutGraphic = base.Context.OwnerGraphic as TextCalloutGraphic;
				if (textCalloutGraphic == null)
					return null;

				return textCalloutGraphic.Callout as UserCalloutGraphic;
			}
		}

		public void Edit()
		{
			UserCalloutGraphic userCallout = this.UserCalloutGraphic;
			if (userCallout != null)
			{
				userCallout.StartEdit();
			}
		}

		public bool Visible
		{
			get { return this.UserCalloutGraphic != null; }
		}

		public event EventHandler VisibleChanged;
	}
}