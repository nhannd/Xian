using System.Collections.Generic;

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
