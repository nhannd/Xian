using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Subclass of DataGridView - overrides the mouse behaviour to allow rows to be "dragged".  This class
    /// is used internally by the <see cref="TableView"/> control and is not intended to be used directly
    /// by application code.
    /// </summary>
    public class DataGridViewWithDragSupport : DataGridView
    {
        private Rectangle _dragBoxFromMouseDown;
        private int _rowIndexFromMouseDown;
        private event EventHandler<ItemDragEventArgs> _itemDrag;

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            _rowIndexFromMouseDown = HitTest(e.X, e.Y).RowIndex;
 
            // call the base class, so that the row gets selected, etc.
            base.OnMouseDown(e);

            if (_rowIndexFromMouseDown > -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                _dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (ShouldBeginDrag(e))
                {
                    // Proceed with the drag and drop, passing in the list item.  
                    ItemDragEventArgs args = new ItemDragEventArgs(MouseButtons.Left, null);
                    EventsHelper.Fire(_itemDrag, this, args);

                    // reset the drag box to empty so that the event is not fired repeatedly
                    _dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
            else
            {
                // allow the base class to handle it only if the left mouse button was not pressed
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            /*                if (this.SelectedRows.Count <= 1)
                            {
                                // call the base class, so that the row gets selected, etc.
                                base.OnMouseUp(e);
                            }
                            _rowIndexFromMouseDown = HitTest(e.X, e.Y).RowIndex;
             */
            base.OnMouseUp(e);
        }

        private bool ShouldBeginDrag(MouseEventArgs e)
        {
            // If the mouse moves outside the rectangle, start the drag.
            return ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                && (_dragBoxFromMouseDown != Rectangle.Empty)
                && !_dragBoxFromMouseDown.Contains(e.X, e.Y);
        }
    }
}
