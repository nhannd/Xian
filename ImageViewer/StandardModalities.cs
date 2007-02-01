using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	public static class StandardModalities
	{
		public static ICollection<string> Modalities
		{
			get { return StandardModalitySettings.Default.ModalitiesAsArray; }
		}
	}
}
