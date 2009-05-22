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

using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class VoiLutManager : IVoiLutManager
	{
		#region Private Fields
		
		private GrayscaleImageGraphic _grayscaleImageGraphic;
		
		#endregion

		public VoiLutManager(GrayscaleImageGraphic grayscaleImageGraphic)
		{
			Platform.CheckForNullReference(grayscaleImageGraphic, "grayscaleImageGraphic");
			_grayscaleImageGraphic = grayscaleImageGraphic;
		}

		#region IVoiLutManager Members

		public IComposableLut GetLut()
		{
			return _grayscaleImageGraphic.VoiLut;
		}

		public void InstallLut(IComposableLut lut)
		{
			IComposableLut existingLut = GetLut();
			if (existingLut is IGeneratedDataLut)
			{
				//Clear the data in the data lut so it's not hanging around using up memory.
				((IGeneratedDataLut)existingLut).Clear();
			}
			
			_grayscaleImageGraphic.InstallVoiLut(lut);
		}

		public bool Invert
		{
			get { return _grayscaleImageGraphic.Invert; }
			set { _grayscaleImageGraphic.Invert = value; }
		}

		public void ToggleInvert()
		{
			_grayscaleImageGraphic.Invert = !_grayscaleImageGraphic.Invert;
		}

		#endregion

		#region IMemorable Members

		public object CreateMemento()
		{
			return new VoiLutMemento(_grayscaleImageGraphic.VoiLut, _grayscaleImageGraphic.Invert);
		}

		public void SetMemento(object memento)
		{
			VoiLutMemento lutMemento = (VoiLutMemento) memento;

			if (_grayscaleImageGraphic.VoiLut != lutMemento.ComposableLutMemento.OriginatingLut)
				this.InstallLut(lutMemento.ComposableLutMemento.OriginatingLut);

			if (lutMemento.ComposableLutMemento.InnerMemento != null)
				_grayscaleImageGraphic.VoiLut.SetMemento(lutMemento.ComposableLutMemento.InnerMemento);

			_grayscaleImageGraphic.Invert = lutMemento.Invert;
		}

		#endregion
	}
}
