using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class ContextMenuControlGraphic : ControlGraphic, IContextMenuProvider
	{
		[CloneCopyReference]
		private IActionSet _actions;

		private string _namespace;
		private string _site;

		public ContextMenuControlGraphic(IGraphic subject)
			: this(string.Empty, string.Empty, null, subject) {}

		public ContextMenuControlGraphic(string site, IActionSet actions, IGraphic subject)
			: this(string.Empty, site, actions, subject) {}

		public ContextMenuControlGraphic(string @namespace, string site, IActionSet actions, IGraphic subject)
			: base(subject)
		{
			_namespace = @namespace;
			_site = site;
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

		public string Namespace
		{
			get
			{
				if (string.IsNullOrEmpty(_namespace))
					return typeof (ContextMenuControlGraphic).FullName;
				return _namespace;
			}
			protected set { _namespace = value; }
		}

		public string Site
		{
			get { return _site; }
			protected set { _site = value; }
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

		protected sealed override IActionSet OnGetExportedActions(string site, IMouseInformation mouseInformation)
		{
			return this.Actions;
		}

		#region IContextMenuProvider Members

		public ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			if (string.IsNullOrEmpty(this.Site))
				return null;

			return ActionModelRoot.CreateModel(this.Namespace, this.Site, this.GetExportedActions(this.Site, mouseInformation));
		}

		#endregion
	}
}