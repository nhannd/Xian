using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	public interface IPresentationStateProvider
	{
		PresentationState PresentationState { get; set; }
	}

	[Cloneable(true)]
	public abstract class PresentationState
	{
		protected PresentationState() {}

		public virtual void Serialize(IPresentationImage image)
		{
			this.Serialize(ToEnumerable(image));
		}

		public abstract void Serialize(IEnumerable<IPresentationImage> images);

		public virtual void Deserialize(IPresentationImage image)
		{
			this.Deserialize(ToEnumerable(image));
		}

		public abstract void Deserialize(IEnumerable<IPresentationImage> images);

		public virtual void Clear(IPresentationImage image)
		{
			this.Clear(ToEnumerable(image));
		}

		public abstract void Clear(IEnumerable<IPresentationImage> image);

		private static IEnumerable<IPresentationImage> ToEnumerable(IPresentationImage image)
		{
			yield return image;
		}
	}
}