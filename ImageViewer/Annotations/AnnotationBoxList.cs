using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An observable container for <see cref="AnnotationBox"/>es.
	/// </summary>
	[Cloneable(true)]
	public sealed class AnnotationBoxList : ObservableList<AnnotationBox>
	{
		internal AnnotationBoxList()
		{
		}

		[CloneInitialize]
		private void Initialize(AnnotationBoxList source, ICloningContext context)
		{
			foreach (AnnotationBox box in source)
				this.Add(box.Clone());
		}
	}
}
