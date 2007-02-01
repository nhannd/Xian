using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class ControlPointGroup : CompositeGraphic, IObservableList<PointF, ControlPointEventArgs>
	{
		private event EventHandler<ControlPointEventArgs> _controlPointChangedEvent;
		private Color _color = Color.Yellow;

		public ControlPointGroup()
		{
		}

		public event EventHandler<ControlPointEventArgs> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		public Color Color
		{
			get { return _color; }
			set 
			{
				_color = value;

				foreach (ControlPoint controlPoint in this.Graphics)
					controlPoint.Color = _color;
			}
		}

		#region IObservableCollection<PointF,ControlPointEventArgs> Members

		public event EventHandler<ControlPointEventArgs> ItemAdded
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		public event EventHandler<ControlPointEventArgs> ItemRemoved
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		public event EventHandler<ControlPointEventArgs> ItemChanged
		{
			add { throw new NotSupportedException(); }
			remove { throw new NotSupportedException(); }
		}

		#endregion

		#region IList<PointF> Members

		public int IndexOf(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Insert(int index, PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveAt(int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public PointF this[int index]
		{
			get
			{
				return ((ControlPoint)this.Graphics[index]).Location;
			}
			set
			{
				((ControlPoint)this.Graphics[index]).Location = value;
			}
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF point)
		{
			int controlPointIndex = this.Count;
			ControlPoint controlPoint = new ControlPoint(controlPointIndex);
			this.Graphics.Add(controlPoint);
			controlPoint.Location = point;
			controlPoint.Color = this.Color;
			controlPoint.ControlPointChanged += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}

		public void Clear()
		{
			this.Graphics.Clear();
		}

		public bool Contains(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void CopyTo(PointF[] array, int arrayIndex)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int Count
		{
			get
			{
				return this.Graphics.Count;
			}
		}

		public bool IsReadOnly
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool Remove(PointF item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<PointF> Members

		public System.Collections.Generic.IEnumerator<PointF> GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool HitTest(Point point)
		{
			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint != null)
					if (controlPoint.HitTest(point))
						return true;
			}

			return false;
		}

		public int HitTestControlPoint(Point point)
		{
			int controlPointIndex = 0;

			// Check if mouse is over a control point
			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint.HitTest(point))
					return controlPointIndex;

				controlPointIndex++;
			}

			return -1;
		}

		private void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			EventsHelper.Fire(_controlPointChangedEvent, this, e);
		}

	}
}
