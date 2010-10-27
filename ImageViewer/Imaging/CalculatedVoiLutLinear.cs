#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Abstract class providing the base functionality for Luts where the <see cref="WindowWidth"/>
	/// and <see cref="WindowCenter"/> are calculated and/or retrieved from an external source.
	/// </summary>
	/// <seealso cref="IVoiLutLinear"/>
	[Cloneable(true)]
	public abstract class CalculatedVoiLutLinear : VoiLutLinearBase, IVoiLutLinear
	{
		#region Protected Constructor

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected CalculatedVoiLutLinear()
		{
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Gets the <see cref="WindowWidth"/>.
		/// </summary>
		protected sealed override double GetWindowWidth()
		{
			return this.WindowWidth;
		}

		/// <summary>
		/// Gets the <see cref="WindowCenter"/>.
		/// </summary>
		protected sealed override double GetWindowCenter()
		{
			return this.WindowCenter;
		}

		#endregion

		#region IVoiLutLinear Members

		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		public abstract double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		public abstract double WindowCenter { get; }

		#endregion
	}
}
