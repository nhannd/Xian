using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	// TODO: Remove IObservableList interface; retain other convenience
	// methods like Add, Remove, etc.

	public class PolyLineAnchorPointsGraphic 
		: CompositeGraphic, IObservableList<PointF, AnchorPointEventArgs>, IMemorable
	{
		private int _numberOfPoints = 0;
		private event EventHandler<AnchorPointEventArgs> _itemAddedEvent;
		private event EventHandler<AnchorPointEventArgs> _itemRemovedEvent;
		private event EventHandler<AnchorPointEventArgs> _anchorPointChangedEvent;
		private Color _color = Color.Yellow;

		public PolyLineAnchorPointsGraphic()
		{
		}

		public event EventHandler<AnchorPointEventArgs> AnchorPointChangedEvent
		{
			add { _anchorPointChangedEvent += value; }
			remove { _anchorPointChangedEvent -= value; }
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_color = value;

				foreach (LinePrimitive line in this.Graphics)
					line.Color = _color;
			}
		}

		#region IObservableCollection<PointF, AnchorPointEventArgs> Members

		public event EventHandler<AnchorPointEventArgs> ItemAdded
		{
			add { _itemAddedEvent += value; }
			remove { _itemAddedEvent -= value; }
		}

		public event EventHandler<AnchorPointEventArgs> ItemRemoved
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
		}

		public event EventHandler<AnchorPointEventArgs> ItemChanged
		{
			add { _itemRemovedEvent += value; }
			remove { _itemRemovedEvent -= value; }
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
			this.Graphics.RemoveAt(index);
		}

		public PointF this[int index]
		{
			get
			{
				if (index == 0)
					return ((LinePrimitive)this.Graphics[0]).Pt1;
				else if (index == 1)
					return ((LinePrimitive)this.Graphics[0]).Pt2;
				else
					return ((LinePrimitive)this.Graphics[index - 1]).Pt2;
			}
			set
			{
				PointF pt;

				if (index == 0)
				{
					pt = ((LinePrimitive)this.Graphics[0]).Pt1;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[0]).Pt1 = value;
						NotifyListeners(0, value);
					}
				}
				else if (index == 1)
				{
					pt = ((LinePrimitive)this.Graphics[0]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[0]).Pt2 = value;
						NotifyListeners(1, value);

						if (this.Count > 2)
							((LinePrimitive)this.Graphics[1]).Pt1 = value;
					}
				}
				else
				{
					pt = ((LinePrimitive)this.Graphics[index - 1]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)this.Graphics[index - 1]).Pt2 = value;
						NotifyListeners(index, value);

						if (index < this.Count - 1)
							((LinePrimitive)this.Graphics[index]).Pt1 = value;
					}
				}
			}
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF point)
		{
			_numberOfPoints++;

			if (this.Count == 1)
			{
				LinePrimitive line = new LinePrimitive();
				line.Color = this.Color;
				this.Graphics.Add(line);
				line.Pt1 = point;
			}
			else if (this.Count == 2)
			{
				((LinePrimitive)this.Graphics[0]).Pt2 = point;
			}
			else
			{
				int previousLineIndex = this.Graphics.Count - 1;
				LinePrimitive previousLine = ((LinePrimitive)this.Graphics[previousLineIndex]);
				LinePrimitive newLine = new LinePrimitive();
				newLine.Color = this.Color;
				this.Graphics.Add(newLine);
				newLine.Pt1 = previousLine.Pt2;
				newLine.Pt2 = point;
			}
		}

		public void Clear()
		{
			this.Graphics.Clear();
			_numberOfPoints = 0;
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
			get { return _numberOfPoints; }
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

		public IEnumerator<PointF> GetEnumerator()
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

		#region IMemorable Members

		public IMemento CreateMemento()
		{
			PointsMemento memento = new PointsMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;

			for (int i = 0; i < this.Count; i++)
				memento.Add(this[i]);

			this.ResetCoordinateSystem();

			return memento;
		}

		public void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			PointsMemento anchorPointsMemento = memento as PointsMemento;
			Platform.CheckForInvalidCast(anchorPointsMemento, "memento", "PointsMemento");

			this.CoordinateSystem = CoordinateSystem.Source;

			int i = 0;

			foreach (PointF point in anchorPointsMemento)
			{
				this[i] = point;
				i++;
			}

			this.ResetCoordinateSystem();
		}

		#endregion

		public override bool HitTest(Point point)
		{
			foreach (LinePrimitive line in this.Graphics)
			{
				if (line.HitTest(point))
					return true;
			}

			return false;
		}

		private void NotifyListeners(int anchorPointIndex, PointF anchorPoint)
		{
			EventsHelper.Fire(_anchorPointChangedEvent, this, new AnchorPointEventArgs(anchorPointIndex, anchorPoint));
		}
	}
}
