#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
