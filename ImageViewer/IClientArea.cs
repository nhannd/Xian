using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	internal interface IClientArea
	{
		Rectangle ClientRectangle
		{
			get;
		}

		Rectangle DrawableClientRectangle
		{
			get;
		}

	}
}
