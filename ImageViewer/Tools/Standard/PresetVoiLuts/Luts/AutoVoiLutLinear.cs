#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	public interface IAutoVoiLutLinear : IVoiLutLinear
	{
		void ApplyNext();
	}

	internal sealed class AutoVoiLutLinear : CalculatedVoiLutLinear, IAutoVoiLutLinear
	{
		private class AutoVoiLutLinearMemento : IMemento, IEquatable<AutoVoiLutLinearMemento>
		{
			public readonly uint Index;

			public AutoVoiLutLinearMemento(uint index)
			{
				this.Index = index;
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is AutoVoiLutLinearMemento)
					return this.Equals((AutoVoiLutLinearMemento) obj);

				return false;	
			}

			#region IEquatable<AutoVoiLutLinearMemento> Members

			public bool Equals(AutoVoiLutLinearMemento other)
			{
				return this.Index == other.Index;
			}

			#endregion
		}

		private readonly ImageSop _imageSop;
		private uint _index;

		public AutoVoiLutLinear(ImageSop imageSop)
		{
			_imageSop = imageSop;
			_index = 0;
		}

		#region Private Properties/Methods

		private Window[] WindowCenterAndWidth
		{
			get { return _imageSop.WindowCenterAndWidth; }
		}

		private void SetIndex(uint newIndex)
		{
			uint lastIndex = _index;
			_index = newIndex;
			if (_index >= this.WindowCenterAndWidth.Length)
				_index = 0;

			if (lastIndex != _index)
				base.OnLutChanged();
		}

		#endregion

		#region IVoiLutLinear Members

		public override double WindowWidth
		{
			get { return this.WindowCenterAndWidth[_index].Width; }
		}

		public override double WindowCenter
		{
			get { return this.WindowCenterAndWidth[_index].Center; }
		}

		#endregion

		#region Public Methods

		#region IAutoVoiLut Members

		public void ApplyNext()
		{
			SetIndex(_index + 1);
		}

		#endregion

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionAutoLinearLut, WindowWidth, WindowCenter);
		}

		public override IMemento CreateMemento()
		{
			return new AutoVoiLutLinearMemento(_index);
		}

		public override void SetMemento(IMemento memento)
		{
			AutoVoiLutLinearMemento autoMemento = memento as AutoVoiLutLinearMemento;
			Platform.CheckForInvalidCast(autoMemento, "memento", typeof(AutoVoiLutLinearMemento).Name);
			this.SetIndex(autoMemento.Index);
		}

		#endregion
	}
}