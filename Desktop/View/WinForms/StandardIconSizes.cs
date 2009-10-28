using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class StandardIconSizes
	{
		public static readonly Size Small = new Size(24, 24);
		public static readonly Size Medium = new Size(32, 32);
		public static readonly Size Large = new Size(48, 48);

		public static Size GetSize(IconSize iconSize)
		{
			if (iconSize == IconSize.Small)
				return Small;
			if (iconSize == IconSize.Medium)
				return Medium;
			return Large;
		}
	}
}
