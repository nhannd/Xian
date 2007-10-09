using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class InformationBox
	{
		private string _data;
		private Point _destinationPoint;
		private bool _visible;

		private event EventHandler _updated;

		public InformationBox()
		{
			_visible = false;
		}

		public event EventHandler Updated
		{
			add { _updated += value; }
			remove { _updated -= value; }
		}

		public string Data
		{
			get { return _data; }
			set
			{
				if (_data == value)
					return;

				_data = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		public Point DestinationPoint
		{
			get { return _destinationPoint; }
			set
			{
				if (value == _destinationPoint)
					return;

				_destinationPoint = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		public bool Visible
		{
			get
			{ return _visible; }
			set
			{
				if (value == _visible)
					return;

				_visible = value;

				EventsHelper.Fire(_updated, this, new EventArgs());
			}
		}

		public void Update(string data, Point destinationPoint)
		{
			bool changed = false;

			if (!_visible || data != _data || destinationPoint != _destinationPoint)
				changed = true;

			_visible = true;
			_data = data;
			_destinationPoint = destinationPoint;
			
			if (changed)
				EventsHelper.Fire(_updated, this, new EventArgs());
		}
	}
}
