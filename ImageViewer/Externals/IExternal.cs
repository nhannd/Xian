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

using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Externals
{
	public interface IExternal : INotifyPropertyChanged
	{
		string Name { get; set; }
		string Label { get; set; }
		bool Enabled { get; set; }
		WindowStyle WindowStyle { get; set; }

		bool IsValid { get; }

		string GetState();
		void SetState(string state);
	}

	public abstract class External : IExternal
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private WindowStyle _windowStyle = WindowStyle.Normal;
		private string _name = string.Empty;
		private string _label = string.Empty;
		private bool _enabled = true;

		protected External() {}

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					this.NotifyPropertyChanged("Name");
				}
			}
		}

		public string Label
		{
			get { return _label; }
			set
			{
				if (_label != value)
				{
					_label = value;
					this.NotifyPropertyChanged("Label");
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					this.NotifyPropertyChanged("Enabled");
				}
			}
		}

		public WindowStyle WindowStyle
		{
			get { return this._windowStyle; }
			set
			{
				if (_windowStyle != value)
				{
					_windowStyle = value;
					this.NotifyPropertyChanged("WindowStyle");
				}
			}
		}

		public virtual bool IsValid
		{
			get { return true; }
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		public abstract string GetState();
		public abstract void SetState(string state);
	}
}