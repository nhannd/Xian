using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	#region Grayscale

	/// <summary>
	/// An extension of <see cref="PresentationLutFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class GrayscalePresentationLutFactory : PresentationLutFactoryBase<GrayscalePresentationLut>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Grayscale";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GrayscalePresentationLutFactory()
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
			get { return "Grayscale"; }
		}
	}

	/// <summary>
	/// A grayscale Presentation Lut.
	/// </summary>
	/// <remarks>
	/// This class should not be instantiated directly, but through the corresponding factory.
	/// </remarks>
	public sealed class GrayscalePresentationLut : PresentationLut
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public GrayscalePresentationLut()
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
		/// Returns a brief description of the Lut.
		/// </summary>
		public override string GetDescription()
		{
			return "Grayscale";
		}
	}

	#endregion

	#region Red

	/// <summary>
	/// An extension of <see cref="PresentationLutFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class RedPresentationLutFactory : PresentationLutFactoryBase<RedPresentationLut>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Red";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public RedPresentationLutFactory()
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
			get { return "Red"; }
		}
	}

	/// <summary>
	/// A red Presentation Lut.
	/// </summary>
	/// <remarks>
	/// This class should not be instantiated directly, but through the corresponding factory.
	/// </remarks>
	public sealed class RedPresentationLut : PresentationLut
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public RedPresentationLut()
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
			return "Red";
		}
	}

	#endregion

	#region Green

	/// <summary>
	/// An extension of <see cref="PresentationLutFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class GreenPresentationLutFactory : PresentationLutFactoryBase<GreenPresentationLut>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Green";

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public GreenPresentationLutFactory()
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
			get { return "Green"; }
		}
	}
	
	/// <summary>
	/// A green Presentation Lut.
	/// </summary>
	public sealed class GreenPresentationLut : PresentationLut
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public GreenPresentationLut()
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
			return "Green";
		}
	}

	#endregion

	#region Blue

	/// <summary>
	/// An extension of <see cref="PresentationLutFactoryExtensionPoint"/>.
	/// </summary>
	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class BluePresentationLutFactory : PresentationLutFactoryBase<BluePresentationLut>
	{
		/// <summary>
		/// Returns the factory name.
		/// </summary>
		public static readonly string FactoryName = "Blue";

		/// <summary>
		/// Default constructor.
		/// </summary>
		public BluePresentationLutFactory()
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
			get { return "Blue"; }
		}
	}

	/// <summary>
	/// A blue Presentation Lut.
	/// </summary>
	public sealed class BluePresentationLut : PresentationLut
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BluePresentationLut()
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
			return "Blue";
		}
	}

	#endregion
}
