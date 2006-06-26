using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// WinForms extension of the <see cref="LayoutToolViewExtensionPoint"/> extension point.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(LayoutToolViewExtensionPoint))]
    public class LayoutToolView : WinFormsView, IToolView
    {
        private LayoutControl _control;
        private LayoutTool _layoutTool;

        public LayoutToolView()
        {
        }

        #region IToolView Members

        public void SetTool(ITool tool)
        {
            _layoutTool = (LayoutTool)tool;
			_layoutTool.LayoutChanged += new EventHandler(OnLayoutToolLayoutChanged);
        }

        #endregion

        public override object GuiElement
        {
            get { return Control; }
        }

        private LayoutControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutControl();
                    _control.ApplyImageBoxesButton.Click += new EventHandler(ApplyImageBoxesButton_Click);
                    _control.ApplyTilesButton.Click += new EventHandler(ApplyTilesButton_Click);
					Synchronize();
                }
                return _control;
            }
        }

        private void Synchronize()
        {
            if (_control != null)
            {
                _control.ImageBoxRows = _layoutTool.ImageBoxRows;
                _control.ImageBoxColumns = _layoutTool.ImageBoxColumns;
                _control.TileRows = _layoutTool.TileRows;
                _control.TileColumns = _layoutTool.TileColumns;
            }
        }

        private void ApplyTilesButton_Click(object sender, EventArgs e)
        {
            _layoutTool.LayoutTiles(Control.TileRows, Control.TileColumns);
        }

        private void ApplyImageBoxesButton_Click(object sender, EventArgs e)
        {
            _layoutTool.LayoutImageBoxes(Control.ImageBoxRows, Control.ImageBoxColumns, Control.TileRows, Control.TileColumns);
        }

        private void OnLayoutToolLayoutChanged(object sender, EventArgs e)
		{
            Synchronize();
        }
    }
}
