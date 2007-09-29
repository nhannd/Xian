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
		public override void Create()
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
		public override void Create()
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
		public override void Create()
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
		public override void Create()
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
