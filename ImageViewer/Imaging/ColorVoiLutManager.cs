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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ColorVoiLutManager : IVoiLutManager
	{
		#region Private Fields
		
		private ColorImageGraphic _colorImageGraphic;
		private bool _enabled = true;
		
		#endregion

		public ColorVoiLutManager(ColorImageGraphic colorImageGraphic)
		{
			Platform.CheckForNullReference(colorImageGraphic, "colorImageGraphic");
			_colorImageGraphic = colorImageGraphic;
		}

		#region IVoiLutManager Members

		/// <summary>
		/// Gets the currently installed VOI LUT.
		/// </summary>
		/// <returns>The VOI LUT as an <see cref="IComposableLut"/>.</returns>
		public IComposableLut GetLut()
		{
			return _colorImageGraphic.VoiLut;
		}

		/// <summary>
		/// Installs a new VOI LUT.
		/// </summary>
		/// <param name="lut">The LUT to be installed.</param>
		public void InstallLut(IComposableLut lut)
		{
			IComposableLut existingLut = GetLut();
			if (existingLut is IGeneratedDataLut)
			{
				//Clear the data in the data lut so it's not hanging around using up memory.
				((IGeneratedDataLut)existingLut).Clear();
			}
			
			_colorImageGraphic.InstallVoiLut(lut);
		}

		/// <summary>
		/// Gets or sets whether the output of the VOI LUT should be inverted for display.
		/// </summary>
		public bool Invert
		{
			get { return _colorImageGraphic.Invert; }
			set { _colorImageGraphic.Invert = value; }
		}

		/// <summary>
		/// Toggles the state of the <see cref="IVoiLutManager.Invert"/> property.
		/// </summary>
		public void ToggleInvert()
		{
			_colorImageGraphic.Invert = !_colorImageGraphic.Invert;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the LUT should be used in rendering the parent object.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Captures the state of an object.
		/// </summary>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.CreateMemento"/> should return an
		/// object containing enough state information so that
		/// when <see cref="IMemorable.SetMemento"/> is called, the object can be restored
		/// to the original state.
		/// </remarks>
		public object CreateMemento()
		{
			return new VoiLutMemento(_colorImageGraphic.VoiLut, _colorImageGraphic.Invert);
		}

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="IMemorable.CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="IMemorable.CreateMemento"/>.
		/// </remarks>
		public void SetMemento(object memento)
		{
			VoiLutMemento lutMemento = (VoiLutMemento) memento;

			if (_colorImageGraphic.VoiLut != lutMemento.ComposableLutMemento.OriginatingLut)
				this.InstallLut(lutMemento.ComposableLutMemento.OriginatingLut);

			if (lutMemento.ComposableLutMemento.InnerMemento != null)
				_colorImageGraphic.VoiLut.SetMemento(lutMemento.ComposableLutMemento.InnerMemento);

			_colorImageGraphic.Invert = lutMemento.Invert;
		}

		#endregion
	}
}
