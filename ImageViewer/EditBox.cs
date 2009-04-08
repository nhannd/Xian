#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
	/// A <see cref="Tile"/> can show a textual input editor
	/// in its client rectangle when one has been supplied.
	/// </summary>
	/// <seealso cref="Tile"/>
	/// <seealso cref="Tile.EditBox"/>
	public class EditBox
	{
		private Point _location = Point.Empty;
		private Size _size = Size.Empty;
		private string _fontName = null;
		private float _fontSize = 12;
		private string _value = string.Empty;
		private bool _multiline = false;
		private string _lastAcceptedValue = string.Empty;

		private event EventHandler _valueAccepted;
		private event EventHandler _valueCancelled;
		private event EventHandler _valueChanged;

		/// <summary>
		/// Constructs a new textual input editor.
		/// </summary>
		public EditBox() {}

		/// <summary>
		/// Constructs a new textual input editor.
		/// </summary>
		/// <param name="initialValue">The initial <see cref="Value">text value</see>.</param>
		public EditBox(string initialValue)
		{
			_value = _lastAcceptedValue = initialValue;
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="Value"/> property has changed since <see cref="EditBox(string)">construction</see> or since the last call to <see cref="Accept"/>, whichever occured later.
		/// </summary>
		public bool IsModified
		{
			get { return _lastAcceptedValue != _value; }
		}

		/// <summary>
		/// Gets the text value at the time of the <see cref="EditBox(string)">construction</see> of the last call to <see cref="Accept"/>, whichever occured later.
		/// </summary>
		public string LastAcceptedValue
		{
			get { return _lastAcceptedValue; }
		}

		/// <summary>
		/// Gets or sets the text value of the editor.
		/// </summary>
		public string Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					this.OnValueChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the location around which the <see cref="EditBox"/> should be centered.
		/// </summary>
		public Point Location
		{
			get { return _location; }
			set { _location = value; }
		}

		/// <summary>
		/// Gets or sets the size that the <see cref="EditBox"/> should take on the client.
		/// </summary>
		public Size Size
		{
			get { return _size; }
			set { _size = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating if this <see cref="EditBox"/> should support multiline input.
		/// </summary>
		public bool Multiline
		{
			get { return _multiline; }
			set { _multiline = value; }
		}

		/// <summary>
		/// Gets or sets the name of the font that the editor should use, or null if the view should use the UI default.
		/// </summary>
		public string FontName
		{
			get { return _fontName; }
			set { _fontName = value; }
		}

		/// <summary>
		/// Gets or sets the size in points of the font that the editor should use.
		/// </summary>
		public float FontSize
		{
			get { return _fontSize; }
			set { _fontSize = value; }
		}

		/// <summary>
		/// Fires when the <see cref="Value"/> property changes.
		/// </summary>
		public event EventHandler ValueChanged
		{
			add { _valueChanged += value; }
			remove { _valueChanged -= value; }
		}

		/// <summary>
		/// Fires when the <see cref="Accept"/> method is invoked.
		/// </summary>
		public event EventHandler ValueAccepted
		{
			add { _valueAccepted += value; }
			remove { _valueAccepted -= value; }
		}

		/// <summary>
		/// Fires when the <see cref="Cancel"/> method is invoked.
		/// </summary>
		public event EventHandler ValueCancelled
		{
			add { _valueCancelled += value; }
			remove { _valueCancelled -= value; }
		}

		/// <summary>
		/// Causes the edit box to accept the current <see cref="Value">value</see>, thereby reseting the <see cref="IsModified"/> and <see cref="LastAcceptedValue"/> properties.
		/// </summary>
		public void Accept()
		{
			_lastAcceptedValue = _value;
			this.OnAccept();
		}

		/// <summary>
		/// Causes the edit box to cancel the current <see cref="Value">value</see> and revert to the <see cref="LastAcceptedValue">last accepted value</see>, thereby reseting the <see cref="IsModified"/>.
		/// </summary>
		public void Cancel()
		{
			_value = _lastAcceptedValue;
			this.OnCancel();
		}

		/// <summary>
		/// Called when the <see cref="Accept"/> method is invoked.
		/// </summary>
		protected virtual void OnAccept()
		{
			EventsHelper.Fire(_valueAccepted, this, new EventArgs());
		}

		/// <summary>
		/// Called when the <see cref="Cancel"/> method is invoked.
		/// </summary>
		protected virtual void OnCancel()
		{
			EventsHelper.Fire(_valueCancelled, this, new EventArgs());
		}

		/// <summary>
		/// Called when the <see cref="Value"/> property changes.
		/// </summary>
		protected virtual void OnValueChanged()
		{
			EventsHelper.Fire(_valueChanged, this, new EventArgs());
		}
	}
}