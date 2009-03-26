using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.GraphicAnnotationSerializers;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IDecoratorGraphic : IGraphic
	{
		/// <summary>
		/// Gets the <see cref="IGraphic"/> decorated by this object.
		/// </summary>
		IGraphic DecoratedGraphic { get; }
	}

	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof (DecoratorGraphicAnnotationSerializer))]
	public abstract class DecoratorCompositeGraphic : CompositeGraphic, IDecoratorGraphic
	{
		[CloneIgnore]
		private IGraphic _decoratedGraphic;

		/// <summary>
		/// Constructs a new <see cref="DecoratorCompositeGraphic"/>.
		/// </summary>
		/// <param name="graphic">The graphic to be decorated.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="graphic"/> is null.</exception>
		protected DecoratorCompositeGraphic(IGraphic graphic)
		{
			Platform.CheckForNullReference(graphic, "graphic");
			base.Graphics.Add(_decoratedGraphic = graphic);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected DecoratorCompositeGraphic(DecoratorCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_decoratedGraphic = CollectionUtils.FirstElement(base.Graphics);
		}

		IGraphic IDecoratorGraphic.DecoratedGraphic
		{
			get { return this.DecoratedGraphic; }
		}

		protected IGraphic DecoratedGraphic
		{
			get { return _decoratedGraphic; }
		}

		public override Roi CreateRoiInformation()
		{
			return this.DecoratedGraphic.CreateRoiInformation();
		}
	}
}