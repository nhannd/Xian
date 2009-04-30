#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
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
			if (_rowIndexFromMouseDown != -1 && 
				(e.Button & MouseButtons.Left) == MouseButtons.Left)
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
