using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class GrayscalePresentationLut : PresentationLut
	{
		public GrayscalePresentationLut(
			int minInputValue, 
			int maxInputValue,
			bool invert) : base(minInputValue, maxInputValue, invert)
		{
			
		}

		protected override void CreateLut()
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

		public override string Name
		{
			get
			{
				return "Grayscale";
			}
		}
	}
}
