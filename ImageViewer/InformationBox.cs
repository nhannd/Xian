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
