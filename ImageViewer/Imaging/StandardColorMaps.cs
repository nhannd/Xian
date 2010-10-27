#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The default (grayscale) color map.
	/// </summary>
	internal sealed class GrayscaleColorMapFactory : ColorMapFactoryBase<GrayscaleColorMap>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Grayscale";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GrayscaleColorMapFactory()
		{
		}

		/// <summary>
		/// Returns the Name of the factory.
		/// </summary>
		public override string Name
		{
			get { return FactoryName; }
		}

		/// <summary>
		/// Returns a brief description of the Factory.
		/// </summary>
		public override string Description
		{
			get { return SR.DescriptionGrayscaleColorMap; }
		}
	}

	/// <summary>
	/// A Grayscale Color Map.
	/// </summary>
	/// <remarks>
	/// This class should not be instantiated directly, but through the corresponding factory.
	/// </remarks>
	internal class GrayscaleColorMap : ColorMap
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GrayscaleColorMap()
			: base()
		{
		}

		/// <summary>
		/// Generates the Lut.
		/// </summary>
		protected override void Create()
		{
			Color color;

			int j = 0;
			int maxGrayLevel = this.Length - 1;
			int min = MinInputValue;
			int max = MaxInputValue;

			for (int i = min; i <= max; i++)
			{
				float scale = j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, value, value, value);
				this[i] = color.ToArgb();
			}
		}

		/// <summary>
		/// Returns an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return SR.DescriptionGrayscaleColorMap;
		}
	}
}