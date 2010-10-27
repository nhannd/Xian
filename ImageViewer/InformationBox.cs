#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A <see cref="Tile"/> can show a textual information box at a given point
	/// in it's client rectangle when one has been supplied.
	/// </summary>
	/// <seealso cref="Tile"/>
	/// <seealso cref="Tile.InformationBox"/>
	public class InformationBox
	{
		private string _data;
		private Point _destinationPoint;
		private bool _visible;

		private event EventHandler _updated;

		/// <summary>
		/// Constructor.
		/// </summary>
		public InformationBox()
		{
			_visible = false;
		}

		/// <summary>
		/// Fires when any of the following properties changes: <see cref="Data"/>, <see cref="Visible"/>, <see cref="DestinationPoint"/>.
		/// </summary>
		public event EventHandler Updated
		{
			add { _updated += value; }
			remove { _updated -= value; }
		}

		/// <summary>
		/// Gets or set the text data that is to be displayed to the user.
		/// </summary>
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

		/// <summary>
		/// The point at which the <see cref="InformationBox"/> should be displayed.
		/// </summary>
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

		/// <summary>
		/// Gets or sets whether or not the <see cref="InformationBox"/> should be visible.
		/// </summary>
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

		/// <summary>
		/// Updates both the <see cref="Data"/> and <see cref="DestinationPoint"/> properties simultaneously.
		/// </summary>
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
