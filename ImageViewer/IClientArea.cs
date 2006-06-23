using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for IClientArea.
	/// </summary>
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
