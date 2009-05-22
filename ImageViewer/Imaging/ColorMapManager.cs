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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ColorMapManager : IColorMapManager
	{
		#region Private Fields

		private GrayscaleImageGraphic _grayscaleImageGraphic;
		
		#endregion

		public ColorMapManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IColorMapManager Members

		public IDataLut GetColorMap()
		{
			return _grayscaleImageGraphic.ColorMap;
		}

		public void InstallColorMap(string name)
		{
			_grayscaleImageGraphic.InstallColorMap(name);
		}

		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			_grayscaleImageGraphic.InstallColorMap(descriptor.Name);
		}

		public void InstallColorMap(IDataLut colorMap)
		{
			_grayscaleImageGraphic.InstallColorMap(colorMap);
		}

		public IEnumerable<ColorMapDescriptor> AvailableColorMaps
		{
			get
			{
				return _grayscaleImageGraphic.AvailableColorMaps;
			}
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento()
		{
			return new ComposableLutMemento(_grayscaleImageGraphic.ColorMap);
		}

		public void SetMemento(object memento)
		{
			ComposableLutMemento lutMemento = (ComposableLutMemento) memento;

			if (_grayscaleImageGraphic.ColorMap != lutMemento.OriginatingLut)
				_grayscaleImageGraphic.InstallColorMap(lutMemento.OriginatingLut as IDataLut);

			if (lutMemento.InnerMemento != null)
				_grayscaleImageGraphic.ColorMap.SetMemento(lutMemento.InnerMemento);
		}

		#endregion
	}
}
