using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class ContextMenuControlGraphic : ControlGraphic
	{
		[CloneCopyReference]
		private IActionSet _actions;

		private string _actionSite;

		public ContextMenuControlGraphic(IGraphic subject)
			: base(subject)
		{
			_actionSite = "";
			_actions = null;
		}

		public ContextMenuControlGraphic(string actionSite, IActionSet actions, IGraphic subject)
			: base(subject)
		{
			_actionSite = actionSite;
			_actions = actions;
		}

		protected ContextMenuControlGraphic(ContextMenuControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public virtual IActionSet Actions
		{
			get { return _actions; }
			set { _actions = value; }
		}

		public virtual string Site
		{
			get { return _actionSite; }
			set { _actionSite = value; }
		}

		protected override ActionModelNode OnGetContextMenuModel(IMouseInformation mouseInformation)
		{
			if (this.Actions == null || string.IsNullOrEmpty(this.Site))
				return null;

			return ActionModelRoot.CreateModel(typeof (ContextMenuControlGraphic).FullName, this.Site, this.Actions);
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			if (mouseInformation.ActiveButton == XMouseButtons.Right)
			{
				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					if (this.HitTest(mouseInformation.Location))
					{
						return true;
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}
			return base.OnMouseStart(mouseInformation);
		}
	}
}