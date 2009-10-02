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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.Standard
{
	[ExtensionPoint]
	public sealed class ToolStripConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolStripConfigurationComponentViewExtensionPoint))]
	public sealed class ToolStripConfigurationComponent : ConfigurationApplicationComponent
	{
		private ToolStripSettingsHelper _settings;
		private bool _wrapLongToolstrips;
		private string _toolStripSize;

		public bool WrapLongToolstrips
		{
			get { return _wrapLongToolstrips; }
			set
			{
				if (_wrapLongToolstrips != value)
				{
					_wrapLongToolstrips = value;
					base.NotifyPropertyChanged("WrapLongToolstrips");
					base.Modified = true;
				}
			}
		}

		public string ToolStripSize
		{
			get { return _toolStripSize; }
			set
			{
				if (_toolStripSize != value)
				{
					_toolStripSize = value;
					base.NotifyPropertyChanged("ToolStripSize");
					base.Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_settings = ToolStripSettingsHelper.Default;
			_wrapLongToolstrips = _settings.WrapLongToolstrips;
			_toolStripSize = Enum.GetName(typeof(ToolStripSizeType), _settings.ToolStripSize);
		}

		public override void Stop()
		{
			_settings = null;

			base.Stop();
		}

		public override void Save()
		{
			_settings.WrapLongToolstrips = _wrapLongToolstrips;
			_settings.ToolStripSize = (ToolStripSizeType)Enum.Parse(typeof(ToolStripSizeType), _toolStripSize);
			_settings.Save();
		}
	}
}