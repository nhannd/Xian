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

#pragma warning disable 0419,1574,1587,1591

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public sealed class AviExportAdvancedComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public class AviCodecInfo
	{
		private readonly Avi.Codec _codec;

		internal AviCodecInfo(Avi.Codec codec)
		{
			_codec = codec;
		}

		internal Avi.Codec Codec
		{
			get { return _codec; }	
		}

		public string Description
		{
			get { return _codec.Description; }
		}

		public bool SupportsQuality
		{
			get { return _codec.SupportsQuality; }
		}
	}

	[AssociateView(typeof(AviExportAdvancedComponentViewExtensionPoint))]
	public class AviExportAdvancedComponent : ApplicationComponent
	{
		private readonly List<AviCodecInfo> _codecInfoList;
		private AviCodecInfo _selectedCodecInfo;
		private bool _useDefaultQuality;
		private int _quality;

		internal AviExportAdvancedComponent(IEnumerable<Avi.Codec> codecs)
		{
			_codecInfoList = CollectionUtils.Map<Avi.Codec, AviCodecInfo>(codecs,
				delegate(Avi.Codec codec){ return new AviCodecInfo(codec); });

			_selectedCodecInfo = _codecInfoList[0];
		}

		internal Avi.Codec SelectedCodec
		{
			get { return _selectedCodecInfo.Codec; }
			set
			{
				AviCodecInfo newValue = _codecInfoList.Find(delegate(AviCodecInfo codecInfo) { return codecInfo.Codec == value; });
				if (newValue == null)
					throw new ArgumentException("The selected codec must be in the codec list.");

				_selectedCodecInfo = newValue;
			}
		}

		public IEnumerable<AviCodecInfo> CodecInfoList
		{
			get { return _codecInfoList; }
		}

		public AviCodecInfo SelectedCodecInfo
		{
			get { return _selectedCodecInfo; }
			set
			{
				if (_selectedCodecInfo == value)
					return;

				if (!_codecInfoList.Contains(_selectedCodecInfo))
					throw new ArgumentException("The selected codec must be in the codec list.");

				_selectedCodecInfo = value;
				if (!_selectedCodecInfo.SupportsQuality)
				    UseDefaultQuality = true;

				NotifyPropertyChanged("UseDefaultQualityEnabled");
			}
		}

		public bool UseDefaultQuality
		{
			get { return _useDefaultQuality; }
			set
			{
				if (_useDefaultQuality == value)
					return;

				_useDefaultQuality = value;
				NotifyPropertyChanged("UseDefaultQuality");
				NotifyPropertyChanged("QualityEnabled");
			}
		}

		public bool UseDefaultQualityEnabled
		{
			get { return SelectedCodec.SupportsQuality; }
		}

		public bool QualityEnabled
		{
			get { return !UseDefaultQuality; }	
		}

		public int MinQuality
		{
			get { return 25; }	
		}

		public int MaxQuality
		{
			get { return 100; }	
		}

		public int Quality
		{
			get { return _quality; }
			set
			{
				if (_quality == value)
					return;

				_quality = value;
				NotifyPropertyChanged("Quality");
			}
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}
