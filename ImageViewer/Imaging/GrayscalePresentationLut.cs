using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class GrayscalePresentationLutFactory : PresentationLutFactoryBase<GrayscalePresentationLut>
	{
		public static readonly string FactoryName = "Grayscale";

		public GrayscalePresentationLutFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return FactoryName; }
		}
	}

	public sealed class GrayscalePresentationLut : PresentationLut
	{
		public GrayscalePresentationLut()
		{
		}

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
	}

	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class RedPresentationLutFactory : PresentationLutFactoryBase<RedPresentationLut>
	{
		public static readonly string FactoryName = "Red";

		public RedPresentationLutFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return FactoryName; }
		}
	}

	public sealed class RedPresentationLut : PresentationLut
	{
		public RedPresentationLut()
			: base()
		{
		}

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
	}

	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class GreenPresentationLutFactory : PresentationLutFactoryBase<GreenPresentationLut>
	{
		public static readonly string FactoryName = "Green";

		public GreenPresentationLutFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return FactoryName; }
		}
	}
	
	public sealed class GreenPresentationLut : PresentationLut
	{
		public GreenPresentationLut()
			: base()
		{
		}

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
	}

	[ExtensionOf(typeof(PresentationLutFactoryExtensionPoint))]
	public sealed class BluePresentationLutFactory : PresentationLutFactoryBase<BluePresentationLut>
	{
		public static readonly string FactoryName = "Blue";

		public BluePresentationLutFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return FactoryName; }
		}
	}

	public sealed class BluePresentationLut : PresentationLut
	{
		public BluePresentationLut()
			: base()
		{
		}

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
	}
}
