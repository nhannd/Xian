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
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class ChangePixelAspectRatioComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (ChangePixelAspectRatioComponentViewExtensionPoint))]
	public class ChangePixelAspectRatioComponent : ApplicationComponent
	{
		private bool _convertWholeDisplaySet;
		private bool _removeCalibration;
		private int _aspectRatioColumn;
		private int _aspectRatioRow;
		private bool _increasePixelDimensions;

		internal ChangePixelAspectRatioComponent()
		{
			_increasePixelDimensions = false;
			_convertWholeDisplaySet = false;
			_removeCalibration = false;
			_aspectRatioRow = 1;
			_aspectRatioColumn = 1;
		}

		[ValidationMethodFor("AspectRatioColumn")]
		private ValidationResult ValidateAspectRatioColumn()
		{
			if (AspectRatioRow == AspectRatioColumn)
				return new ValidationResult(false, "You must specify a non-1:1 aspect ratio");

			return new ValidationResult(true, "");
		}

		[ValidationMethodFor("AspectRatioRow")]
		private ValidationResult ValidateAspectRatioRow()
		{
			if (AspectRatioRow == AspectRatioColumn)
				return new ValidationResult(false, "You must specify a non-1:1 aspect ratio");

			return new ValidationResult(true, "");
		}

		public bool IncreasePixelDimensions
		{
			get { return _increasePixelDimensions; }
			set
			{
				if (_increasePixelDimensions == value)
					return;

				_increasePixelDimensions = value;
				NotifyPropertyChanged("IncreasePixelDimensions");
			}
		}
		public bool RemoveCalibration
		{
			get { return _removeCalibration; }
			set
			{
				if (_removeCalibration == value)
					return;

				_removeCalibration = value;
				NotifyPropertyChanged("RemoveCalibration");
			}
		}

		public int AspectRatioRow
		{
			get { return _aspectRatioRow; }
			set
			{
				if (_aspectRatioRow == value)
					return;

				_aspectRatioRow = value;
				NotifyPropertyChanged("AspectRatioRow");
			}
		}

		public int AspectRatioColumn
		{
			get { return _aspectRatioColumn; }
			set
			{
				if (_aspectRatioColumn == value)
					return;

				_aspectRatioColumn = value;
				NotifyPropertyChanged("AspectRatioColumn");
			}
		}

		public bool ConvertWholeDisplaySet
		{
			get { return _convertWholeDisplaySet; }
			set
			{
				if (_convertWholeDisplaySet == value)
					return;

				_convertWholeDisplaySet = value;
				NotifyPropertyChanged("ConvertWholeDisplaySet");
			}
		}

		public void Accept()
		{
			if (base.HasValidationErrors)
			{
				base.ShowValidation(true);	
				return;
			}

			base.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}

		public void Cancel()
		{
			base.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}
	}
}