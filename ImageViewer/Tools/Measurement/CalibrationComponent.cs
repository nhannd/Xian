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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	/// <summary>
	/// Extension point for views onto <see cref="CalibrationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class CalibrationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CalibrationComponent class.
	/// </summary>
	[AssociateView(typeof(CalibrationComponentViewExtensionPoint))]
	public class CalibrationComponent : ApplicationComponent
	{
		private double _lengthInCm;
		private static readonly int _decimalPlaces = 1;
		private static readonly double _minimum = 0.1;
		private static readonly double _increment = 0.1;

		public CalibrationComponent()
		{
			_lengthInCm = 1.0;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public CalibrationComponent(double lengthInCm)
		{
			Platform.CheckPositive(lengthInCm, "lengthInCm");

			_lengthInCm = Math.Round(lengthInCm, _decimalPlaces);
		}

		[ValidateGreaterThan(0.0, Inclusive = false, Message = "MessageInvalidLength")]
		public double LengthInCm
		{
			get { return _lengthInCm; }
			set { _lengthInCm = value; }
		}

		public int DecimalPlaces
		{
			get { return _decimalPlaces; }
		}

		public double Minimum
		{
			get { return _minimum; }
		}

		public double Increment
		{
			get { return _increment; }
		}

		public void Accept()
		{
			if (base.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

	}
}
