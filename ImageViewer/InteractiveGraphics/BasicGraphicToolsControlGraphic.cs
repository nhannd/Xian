using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class BasicGraphicToolsControlGraphic : ContextMenuControlGraphic
	{
		[CloneCopyReference]
		private GraphicToolExtensionPoint _extensionPoint = new GraphicToolExtensionPoint();

		public BasicGraphicToolsControlGraphic(IGraphic subject)
			: base("basicgraphic-menu", null, subject) {}

		protected BasicGraphicToolsControlGraphic(BasicGraphicToolsControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public override IActionSet Actions
		{
			get { return new ToolSet(_extensionPoint, new GraphicToolContext(this.DecoratedGraphic, this.Subject, this.ImageViewer.DesktopWindow)).Actions; }
			set { }
		}
	}
}