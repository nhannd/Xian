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
using System.Windows.Forms;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides data for the item dropped events.
    /// </summary>
	public class ListBoxItemDroppedEventArgs : EventArgs
	{
    	private readonly int _draggedIndex;
    	private readonly int _droppedIndex;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal ListBoxItemDroppedEventArgs(int draggedIndex, int droppedIndex)
		{
			_draggedIndex = draggedIndex;
			_droppedIndex = droppedIndex;
		}

		/// <summary>
		/// Gets the index of the item that was dragged.
		/// </summary>
		public int DraggedIndex
		{
			get { return _draggedIndex; }
		}

		/// <summary>
		/// Gets the index of the item being dropped on.
		/// </summary>
    	public int DroppedIndex
    	{
			get { return _droppedIndex; }
    	}
	}

    /// <summary>
	/// Subclass of ListBox - overrides the mouse behaviour to allow items to be "dragged".  This class
	/// is used internally by the <see cref="ListBoxView"/> control and is not intended to be used directly
    /// by application code.
    /// </summary>
    public class ListBoxWithDragSupport : ListBox
	{
		private int _itemIndexFromMouseDown;
		private Rectangle _dragBoxFromMouseDown;

		private event EventHandler<ListBoxItemDroppedEventArgs> _itemDropped;

    	/// <summary>
    	/// Occurs when an item in the collection is dragged and dropped.
    	/// </summary>
		public event EventHandler<ListBoxItemDroppedEventArgs> ItemDropped
		{
			add { _itemDropped += value; }
			remove { _itemDropped -= value; }
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// We need to store the _itemIndexFromMouseDown here because there is a known problem in .Net
			// calling DoDragDrop in MouseDown will kill the SelectedIndexChanged event
			_itemIndexFromMouseDown = this.IndexFromPoint(e.Location);

			// call the base class, so that the item gets selected, etc.
			base.OnMouseDown(e);

			if (_itemIndexFromMouseDown > -1)
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
			if (_itemIndexFromMouseDown != -1 && (e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				if (ShouldBeginDrag(e))
				{
					// Proceed with the drag and drop.  
					this.DoDragDrop(_itemIndexFromMouseDown, DragDropEffects.All);

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

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof(int)))
				drgevent.Effect = DragDropEffects.Move;

			base.OnDragOver(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof(int)))
			{
				int index = (int)drgevent.Data.GetData(typeof(int));
				int newIndex = this.IndexFromPoint(this.PointToClient(new Point(drgevent.X, drgevent.Y)));

				// Notify subscriber that an item is dropped.
				EventsHelper.Fire(_itemDropped, this, new ListBoxItemDroppedEventArgs(index, newIndex));

				// Set selection on the new item.
				this.SetSelected(newIndex, true);
			}

			base.OnDragDrop(drgevent);
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
