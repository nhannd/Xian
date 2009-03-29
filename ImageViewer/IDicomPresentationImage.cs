using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer {
	/// <summary>
	/// The interface for all DICOM-based Presentation Images.
	/// </summary>
	public interface IDicomPresentationImage :
		IPresentationImage,
		IImageSopProvider,
		IAnnotationLayoutProvider,
		IImageGraphicProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		ISpatialTransformProvider,
		IDicomSoftcopyPresentationStateProvider
		
	{
		/// <summary>
		/// Gets this presentation image's collection of domain-level graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </remarks>
		GraphicCollection DicomGraphics { get; }
	}
}
