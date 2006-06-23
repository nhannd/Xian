using System;
using System.Drawing;

namespace ClearCanvas.Workstation.Model
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
