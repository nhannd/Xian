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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts
{
	[Cloneable(true)]
	internal sealed class PresetVoiLutLinear : CalculatedVoiLutLinear
	{
		[Cloneable(true)]
		public sealed class PresetVoiLutLinearParameters : IEquatable<PresetVoiLutLinearParameters>
		{
			public readonly string Name;
			public readonly double WindowWidth;
			public readonly double WindowCenter;

			public PresetVoiLutLinearParameters()
			{
			}

			public PresetVoiLutLinearParameters(string name, double windowWidth, double windowCenter)
			{
				Platform.CheckForEmptyString(name, "name");

				if (double.IsNaN(windowWidth) || windowWidth < 1)
					throw new ArgumentException(String.Format(SR.ExceptionFormatWindowWidthInvalid, windowWidth));
				if (double.IsNaN(windowCenter))
					throw new ArgumentException(SR.ExceptionWindowCenterInvalid);
				if (String.IsNullOrEmpty(name))
					throw new ArgumentException(SR.ExceptionPresetNameCannotBeEmpty);

				this.Name = name;
				this.WindowWidth = windowWidth;
				this.WindowCenter = windowCenter;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				if (obj is PresetVoiLutLinearParameters)
					return this.Equals((PresetVoiLutLinearParameters) obj);

				return false;
			}

			#region IEquatable<PresetVoiLutLinearParameters> Members

			public bool Equals(PresetVoiLutLinearParameters other)
			{
				if (other == null)
					return false;

				return Name == other.Name && WindowWidth == other.WindowWidth && WindowCenter == other.WindowCenter;
			}

			#endregion
		}

		private PresetVoiLutLinearParameters _parameters;

		public PresetVoiLutLinear(PresetVoiLutLinearParameters parameters)
		{
			_parameters = parameters;
		}

		private PresetVoiLutLinear()
		{
		}

		#region Public Properties

		public PresetVoiLutLinearParameters Parameters
		{
			get { return _parameters; }
			set
			{
				Platform.CheckForNullReference(value, "Parameters");

				if (_parameters.Equals(value))
					return;

				_parameters = value;
				base.OnLutChanged();
			}
		}

		public string Name
		{
			get { return _parameters.Name; }
		}

		public override double WindowWidth
		{
			get { return _parameters.WindowWidth; }
		}

		public override double WindowCenter
		{
			get { return _parameters.WindowCenter; }
		}

		#endregion

		#region Public Methods

		public override string GetDescription()
		{
			return String.Format(SR.FormatDescriptionPresetLinearLut, WindowWidth, WindowCenter, this.Name);
		}

		public override object CreateMemento()
		{
			return _parameters;
		}

		public override void SetMemento(object memento)
		{
			PresetVoiLutLinearParameters parameters = (PresetVoiLutLinearParameters) memento;
			this.Parameters = parameters;
		}

		#endregion
	}
}