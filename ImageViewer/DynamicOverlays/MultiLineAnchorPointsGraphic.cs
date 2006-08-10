using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for AnchorPoints.
	/// </summary>
	public class MultiLineAnchorPointsGraphic 
		: Graphic, IObservableList<PointF, AnchorPointEventArgs>, IMemorable
	{
		private int _NumberOfPoints = 0;
		private event EventHandler<AnchorPointEventArgs> _ItemAddedEvent;
		private event EventHandler<AnchorPointEventArgs> _ItemRemovedEvent;
		private event EventHandler<AnchorPointEventArgs> _AnchorPointChangedEvent;

		public MultiLineAnchorPointsGraphic()
		{
		}

		public event EventHandler<AnchorPointEventArgs> AnchorPointChangedEvent
		{
			add { _AnchorPointChangedEvent += value; }
			remove { _AnchorPointChangedEvent -= value; }
		}
		
		#region IObservableCollection<PointF, AnchorPointEventArgs> Members

		public event EventHandler<AnchorPointEventArgs> ItemAdded
		{
			add { _ItemAddedEvent += value; }
			remove { _ItemAddedEvent -= value; }
		}

		public event EventHandler<AnchorPointEventArgs> ItemRemoved
		{
			add { _ItemRemovedEvent += value; }
			remove { _ItemRemovedEvent -= value; }
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
			base.Graphics.RemoveAt(index);
		}

		public PointF this[int index]
		{
			get
			{
				if (index == 0)
					return ((LinePrimitive)base.Graphics[0]).Pt1;
				else if (index == 1)
					return ((LinePrimitive)base.Graphics[0]).Pt2;
				else
					return ((LinePrimitive)base.Graphics[index - 1]).Pt2;
			}
			set
			{
				PointF pt;

				if (index == 0)
				{
					pt = ((LinePrimitive)base.Graphics[0]).Pt1;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)base.Graphics[0]).Pt1 = value;
						NotifyListeners(0, value);
					}
				}
				else if (index == 1)
				{
					pt = ((LinePrimitive)base.Graphics[0]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)base.Graphics[0]).Pt2 = value;
						NotifyListeners(1, value);

						if (this.Count > 2)
							((LinePrimitive)base.Graphics[1]).Pt1 = value;
					}
				}
				else
				{
					pt = ((LinePrimitive)base.Graphics[index - 1]).Pt2;

					if (!FloatComparer.AreEqual(pt, value))
					{
						((LinePrimitive)base.Graphics[index - 1]).Pt2 = value;
						NotifyListeners(index, value);

						if (index < this.Count - 1)
							((LinePrimitive)base.Graphics[index]).Pt1 = value;
					}
				}
			}
		}

		#endregion

		#region ICollection<PointF> Members

		public void Add(PointF point)
		{
			_NumberOfPoints++;

			if (this.Count == 1)
			{
				LinePrimitive line = new LinePrimitive();
				base.Graphics.Add(line);
				line.Pt1 = point;
			}
			else if (this.Count == 2)
			{
				((LinePrimitive)base.Graphics[0]).Pt2 = point;
			}
			else
			{
				int previousLineIndex = base.Graphics.Count - 1;
				LinePrimitive previousLine = ((LinePrimitive)base.Graphics[previousLineIndex]);
				LinePrimitive newLine = new LinePrimitive();
				base.Graphics.Add(newLine);
				newLine.Pt1 = previousLine.Pt2;
				newLine.Pt2 = point;
			}
		}

		public void Clear()
		{
			base.Graphics.Clear();
			_NumberOfPoints = 0;
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
			get { return _NumberOfPoints; }
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

		public override bool HitTest(XMouseEventArgs e)
		{
			foreach (LinePrimitive line in this.Graphics)
			{
				if (line.HitTest(e))
					return true;
			}

			return false;
		}

		private void NotifyListeners(int anchorPointIndex, PointF anchorPoint)
		{
			EventsHelper.Fire(_AnchorPointChangedEvent, this, new AnchorPointEventArgs(anchorPointIndex, anchorPoint));
		}
	}
}
