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
	public sealed class ToolbarConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolbarConfigurationComponentViewExtensionPoint))]
	public sealed class ToolbarConfigurationComponent : ConfigurationApplicationComponent
	{
		private bool _wrap;
		private IconSize _iconSize;

		public bool Wrap
		{
			get { return _wrap; }
			set
			{
				if (_wrap != value)
				{
					_wrap = value;
					base.NotifyPropertyChanged("Wrap");
					base.Modified = true;
				}
			}
		}

		public IconSize IconSize
		{
			get { return _iconSize; }
			set
			{
				if (_iconSize != value)
				{
					_iconSize = value;
					base.NotifyPropertyChanged("IconSize");
					base.Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			_wrap = ToolStripSettings.Default.WrapLongToolstrips;
			_iconSize = ToolStripSettings.Default.IconSize;
		}

		public override void Save()
		{
			ToolStripSettings.Default.WrapLongToolstrips = _wrap;
			ToolStripSettings.Default.IconSize = _iconSize;
			ToolStripSettings.Default.Save();
		}
	}
}