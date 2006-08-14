using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for RulerOverlay.
	/// </summary>
	public class ControlPointsGraphic : Graphic, IObservableList<PointF, ControlPointEventArgs>
	{
		private event EventHandler<ControlPointEventArgs> _controlPointChangedEvent;

		public ControlPointsGraphic()
		{
		}

		public event EventHandler<ControlPointEventArgs> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
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
				return ((ControlPoint)base.Graphics[index]).Location;
			}
			set
			{
				((ControlPoint)base.Graphics[index]).Location = value;
			}
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF point)
		{
			int controlPointIndex = this.Count;
			ControlPoint controlPoint = new ControlPoint(controlPointIndex);
			base.Graphics.Add(controlPoint);
			controlPoint.Location = point;
			controlPoint.ControlPointChanged += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}

		public void Clear()
		{
			base.Graphics.Clear();
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
				return base.Graphics.Count;
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

		public override bool HitTest(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");

			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint != null)
					if (controlPoint.HitTest(e))
						return true;
			}

			return false;
		}

		public int HitTestControlPoint(XMouseEventArgs e)
		{
			Platform.CheckForNullReference(e, "e");
			int controlPointIndex = 0;

			// Check if mouse is over a control point
			foreach (ControlPoint controlPoint in this.Graphics)
			{
				if (controlPoint.HitTest(e))
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
