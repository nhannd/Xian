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

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
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