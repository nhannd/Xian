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

using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	#region Grayscale

	/// <summary>
	/// An extension of <see cref="ColorMapFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(ColorMapFactoryExtensionPoint))]
	public sealed class GrayscaleColorMapFactory : ColorMapFactoryBase<GrayscaleColorMap>
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
	public sealed class GrayscaleColorMap : ColorMap
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
			uint maxGrayLevel = this.Length - 1;

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

	#endregion

	// TODO: Get rid of all colormaps except grayscale

	#region Red

	/// <summary>
	/// An extension of <see cref="ColorMapFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(ColorMapFactoryExtensionPoint))]
	public sealed class RedColorMapFactory : ColorMapFactoryBase<RedColorMap>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Red";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public RedColorMapFactory()
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
			get { return SR.DescriptionRedColorMap; }
		}
	}

	/// <summary>
	/// A red Color Map.
	/// </summary>
	/// <remarks>
	/// This class should not be instantiated directly, but through the corresponding factory.
	/// </remarks>
	public sealed class RedColorMap : ColorMap
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public RedColorMap()
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
			uint maxGrayLevel = this.Length - 1;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, value, 0, 0);
				this[i] = color.ToArgb();
			}
		}

		/// <summary>
		/// Returns an abbreviated description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return SR.DescriptionRedColorMap;
		}
	}

	#endregion

	#region Green

	/// <summary>
	/// An extension of <see cref="ColorMapFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(ColorMapFactoryExtensionPoint))]
	public sealed class GreenColorMapFactory : ColorMapFactoryBase<GreenColorMap>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Green";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GreenColorMapFactory()
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
		/// Returns a brief description of the factory.
		/// </summary>
		public override string Description
		{
			get { return SR.DescriptionGreenColorMap; }
		}
	}
	
	/// <summary>
	/// A green Color Map.
	/// </summary>
	public sealed class GreenColorMap : ColorMap
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public GreenColorMap()
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
			uint maxGrayLevel = this.Length - 1;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, 0, value, 0);
				this[i] = color.ToArgb();
			}
		}

		/// <summary>
		/// Returns a brief description of the Lut.
		/// </summary>
		/// <returns></returns>
		public override string GetDescription()
		{
			return SR.DescriptionGreenColorMap;
		}
	}

	#endregion

	#region Blue

	/// <summary>
	/// An extension of <see cref="ColorMapFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(ColorMapFactoryExtensionPoint))]
	public sealed class BlueColorMapFactory : ColorMapFactoryBase<BlueColorMap>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Blue";

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BlueColorMapFactory()
		{
		}

		/// <summary>
		/// Returns the name of the factory.
		/// </summary>
		public override string Name
		{
			get { return FactoryName; }
		}

		/// <summary>
		/// Returns a brief description of the factory.
		/// </summary>
		public override string Description
		{
			get { return SR.DescriptionBlueColorMap; }
		}
	}

	/// <summary>
	/// A blue Color Map.
	/// </summary>
	public sealed class BlueColorMap : ColorMap
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BlueColorMap()
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
			uint maxGrayLevel = this.Length - 1;

			for (int i = this.MinInputValue; i <= this.MaxInputValue; i++)
			{
				float scale = (float)j / (float)maxGrayLevel;
				j++;

				int value = (int)(byte.MaxValue * scale);
				color = Color.FromArgb(255, 0, 0, value);
				this[i] = color.ToArgb();
			}
		}

		/// <summary>
		/// Returns a brief description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return SR.DescriptionBlueColorMap;
		}
	}

	#endregion
}
