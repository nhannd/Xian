using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	//TODO: turn this into a graphic decorator, where the site can be specified.  Will allow much finer control of the graphic menus.
	[Cloneable]
	public class AnnotationGraphic : StandardStatefulInteractiveGraphic, IContextMenuProvider
	{
		[CloneIgnore]
		private InteractiveGraphic _subjectGraphic;
		[CloneIgnore]
		private ToolSet _toolSet;

		public AnnotationGraphic(InteractiveGraphic subjectGraphic)
			: base(subjectGraphic)
		{
			_subjectGraphic = subjectGraphic;
			Initialize();
		}

		protected AnnotationGraphic(AnnotationGraphic source, ICloningContext context)
			: base(source, context)
		{
		}

		private void Initialize()
		{
			if (!base.Graphics.Contains(_subjectGraphic))
				base.Graphics.Add(_subjectGraphic);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_subjectGraphic = CollectionUtils.SelectFirst(base.Graphics,
											  delegate(IGraphic test) { return test is InteractiveGraphic; }) as InteractiveGraphic;

			Initialize();
			Platform.CheckForNullReference(_subjectGraphic, "_subjectGraphic");

			//the roi and callout may have been selected, so we force a state change
			this.State = this.CreateInactiveState();
		}

		/// <summary>
		/// Gets the <see cref="InteractiveGraphic"/> that defines
		/// the subject.
		/// </summary>
		public InteractiveGraphic Subject
		{
			get { return _subjectGraphic; }
		}

		public override bool HitTest(System.Drawing.Point point)
		{
			return Subject.HitTest(point);
		}

		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			if (_subjectGraphic.HitTest(mouseInformation.Location))
				return CreateContextMenuModel(_subjectGraphic);

			return null;
		}

		protected ActionModelNode CreateContextMenuModel(IGraphic graphic)
		{
			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this, graphic, this.ImageViewer.DesktopWindow));

			return ActionModelRoot.CreateModel(typeof(AnnotationGraphic).FullName, "basicgraphic-menu", _toolSet.Actions);
		}
	}
}