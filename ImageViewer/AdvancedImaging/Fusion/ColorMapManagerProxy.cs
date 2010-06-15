#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal class ColorMapManagerProxy : IColorMapManager, ILayerOpacityManager
	{
		[CloneIgnore]
		private readonly XColorMapInstaller _placeholderColorMapInstaller;

		[CloneIgnore]
		private readonly IColorMapManager _placeholderColorMapManager;

		[CloneIgnore]
		private IColorMapManager _realColorMapManager;

		public ColorMapManagerProxy()
		{
			_placeholderColorMapManager = new ColorMapManager(_placeholderColorMapInstaller = new XColorMapInstaller());
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ColorMapManagerProxy(ColorMapManagerProxy source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_placeholderColorMapManager = new ColorMapManager(_placeholderColorMapInstaller = source._placeholderColorMapInstaller.Clone());
		}

		public void SetRealColorMapManager(IColorMapManager realColorMapManager)
		{
			_realColorMapManager = realColorMapManager;
			InstallColorMap();
		}

		private void InstallColorMap()
		{
			if (_realColorMapManager != null)
				_realColorMapManager.InstallColorMap(_placeholderColorMapManager.ColorMap);
		}

		#region ILayerOpacityManager Members

		public bool Enabled
		{
			get { return true; }
			set { }
		}

		public float Opacity
		{
			get { return _placeholderColorMapInstaller.Opacity; }
			set
			{
				Platform.CheckTrue(value >= 0f && value <= 1f, "Opacity must be between 0 and 1.");
				if (_placeholderColorMapInstaller.Opacity != value)
				{
					_placeholderColorMapInstaller.Opacity = value;
					InstallColorMap();
				}
			}
		}

		public bool Thresholding
		{
			get { return _placeholderColorMapInstaller.Thresholding; }
			set
			{
				if (_placeholderColorMapInstaller.Thresholding != value)
				{
					_placeholderColorMapInstaller.Thresholding = value;
					InstallColorMap();
				}
			}
		}

		#endregion

		#region IColorMapManager Members

		[Obsolete("Use the ColorMap property instead.")]
		IDataLut IColorMapManager.GetColorMap()
		{
			return _placeholderColorMapManager.GetColorMap();
		}

		#endregion

		#region IColorMapInstaller Members

		IDataLut IColorMapInstaller.ColorMap
		{
			get { return _placeholderColorMapManager.ColorMap; }
		}

		void IColorMapInstaller.InstallColorMap(string name)
		{
			_placeholderColorMapManager.InstallColorMap(name);
			InstallColorMap();
		}

		void IColorMapInstaller.InstallColorMap(ColorMapDescriptor descriptor)
		{
			_placeholderColorMapManager.InstallColorMap(descriptor);
			InstallColorMap();
		}

		void IColorMapInstaller.InstallColorMap(IDataLut colorMap)
		{
			_placeholderColorMapManager.InstallColorMap(colorMap);
			InstallColorMap();
		}

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get { return _placeholderColorMapManager.AvailableColorMaps; }
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento()
		{
			return _placeholderColorMapManager.CreateMemento();
		}

		public void SetMemento(object memento)
		{
			_placeholderColorMapManager.SetMemento(memento);
			InstallColorMap();
		}

		#endregion

		#region XColorMapInstaller Class

		private class XColorMapInstaller : IColorMapInstaller
		{
			private IDataLut _alphaColorMap;
			private IDataLut _colorMap;
			private string _colorMapName = HotIronColorMapFactory.ColorMapName;
			private bool _thresholding = false;
			private float _opacity = 0.5f;

			public XColorMapInstaller() {}

			public XColorMapInstaller Clone()
			{
				var clone = new XColorMapInstaller();
				clone._alphaColorMap = _alphaColorMap != null ? (IDataLut) _alphaColorMap.Clone() : null;
				clone._colorMap = _colorMap != null ? (IDataLut) _colorMap.Clone() : null;
				clone._colorMapName = _colorMapName;
				clone._thresholding = _thresholding;
				clone._opacity = _opacity;
				return clone;
			}

			public bool Thresholding
			{
				get { return _thresholding; }
				set
				{
					if (_thresholding != value)
					{
						_thresholding = value;
						_alphaColorMap = null;
					}
				}
			}

			public float Opacity
			{
				get { return _opacity; }
				set
				{
					if (_opacity != value)
					{
						_opacity = value;
						_alphaColorMap = null;
					}
				}
			}

			public IDataLut ColorMap
			{
				get
				{
					if (_alphaColorMap == null)
					{
						if (!string.IsNullOrEmpty(_colorMapName))
							_alphaColorMap = AlphaColorMapFactory.GetColorMap(_colorMapName, (byte) (byte.MaxValue*_opacity), _thresholding);
						else if (_colorMap != null)
							_alphaColorMap = AlphaColorMapFactory.GetColorMap(_colorMap, (byte) (byte.MaxValue*_opacity), _thresholding);
					}
					return _alphaColorMap;
				}
			}

			public void InstallColorMap(string name)
			{
				if (_colorMapName != name)
				{
					_colorMapName = name;
					_colorMap = null;
					_alphaColorMap = null;
				}
			}

			public void InstallColorMap(ColorMapDescriptor descriptor)
			{
				this.InstallColorMap(descriptor.Name);
			}

			public void InstallColorMap(IDataLut colorMap)
			{
				if (_colorMap != colorMap)
				{
					_colorMap = colorMap;
					_colorMapName = null;
					_alphaColorMap = null;
				}
			}

			public IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get
				{
					using (var lutFactory = LutFactory.Create())
					{
						return lutFactory.AvailableColorMaps;
					}
				}
			}
		}

		#endregion
	}
}