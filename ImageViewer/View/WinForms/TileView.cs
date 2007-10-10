using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TileComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TileViewExtensionPoint))]
    public class TileView : WinFormsView, IView
    {
        private Tile _tile;
        private TileControl _tileControl;
		private Rectangle _parentRectangle;
		private int _parentImageBoxInsetWidth;

		public Tile Tile
		{
			get { return _tile; }
			set { _tile = value; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set { _parentRectangle = value; }
		}

		public int ParentImageBoxInsetWidth
		{
			get { return _parentImageBoxInsetWidth; }
			set { _parentImageBoxInsetWidth = value; }
		}

		public override object GuiElement
        {
            get
            {
                if (_tileControl == null)
                {
                    _tileControl = new TileControl(this.Tile, this.ParentRectangle, this.ParentImageBoxInsetWidth);
                }
                return _tileControl;
            }
        }
    }
}
