using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class GrayscaleImageGraphic : IndexedImageGraphic, IVoiLutLinearProvider
	{
		#region Private fields

		private LutComposer _lutComposer;
		private LutFactory _lutFactory;
		
		private int _minPixelValue;
		private int _maxPixelValue;

		#endregion

		#region Public constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// <para>
		/// Creates an empty grayscale image of a specific size.
		/// By default, all pixels are set to zero (i.e., black). 
		/// Useful as a canvas on which pixels can be set by the client.
		/// </para>
		/// <para>
		/// By default, the image is 16-bit unsigned with
		/// <i>bits stored = 16</i>, <i>high bit = 15</i>,
		/// <i>rescale slope = 1.0</i> and <i>rescale intercept = 0.0</i>.
		/// </para>
		/// </remarks>
		public GrayscaleImageGraphic(int rows, int columns)
			: base(rows, 
				   columns, 
				   16, /* bits allocated */
				   16, /* bits stored */
				   15, /* high bit */
				   false) /* is signed */
		{
			Initialize(false, 1.0, 0.0);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated">Can be 8 or 16.</param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelData"></param>
		/// <remarks>
		/// Creates an grayscale image using existing pixel data.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				pixelData)
		{
			Initialize(inverted, rescaleSlope, rescaleIntercept);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="GrayscaleImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="inverted"></param>
		/// <param name="rescaleSlope"></param>
		/// <param name="rescaleIntercept"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public GrayscaleImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			PixelDataGetter pixelDataGetter)
			: base(
				rows,
				columns,
				bitsAllocated,
				bitsStored,
				highBit,
				isSigned,
				pixelDataGetter)
		{
			Initialize(inverted, rescaleSlope, rescaleIntercept);
		}

		private void Initialize(bool inverted, double rescaleSlope, double rescaleIntercept)
		{
			_minPixelValue = int.MinValue;
			_maxPixelValue = int.MaxValue;

			InstallGrayscaleLuts(rescaleSlope, rescaleIntercept, inverted);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Returns the minimum pixel value in the image pixel data itself.  
		/// </summary>
		/// <remarks>
		/// Note that on first calling the get method
		/// of this property, both the minimum and maximum pixel values will be calculated, after which they are cached
		/// for performance reasons.  So, if the pixel data in this image is variable, the minimum and maximum values 
		/// must also be updated in order to remain correct.		
		/// </remarks>
		public virtual int MinPixelValue
		{
			get
			{
				if (_minPixelValue == int.MinValue)
					((IndexedPixelData)this.PixelData).CalculateMinMaxPixelValue(out _minPixelValue, out _maxPixelValue);

				return _minPixelValue;
			}
			protected set
			{
				Platform.CheckArgumentRange(
					value, 
					((IndexedPixelData)this.PixelData).AbsoluteMinPixelValue, 
					((IndexedPixelData)this.PixelData).AbsoluteMaxPixelValue, "value");

				_minPixelValue = value;
			}
		}

		/// <summary>
		/// Returns the maximum pixel value in the image pixel data itself.
		/// </summary>
		/// <remarks>
		/// Note that on first calling the get method
		/// of this property, both the minimum and maximum pixel values will be calculated, after which they are cached
		/// for performance reasons.  So, if the pixel data in this image is variable, the minimum and maximum values 
		/// must also be updated in order to remain correct.
		/// </remarks>
		public virtual int MaxPixelValue
		{
			get
			{
				if (_maxPixelValue == int.MaxValue)
					((IndexedPixelData)this.PixelData).CalculateMinMaxPixelValue(out _minPixelValue, out _maxPixelValue);

				return _maxPixelValue;
			}
			protected set
			{
				Platform.CheckArgumentRange(
					value, 
					((IndexedPixelData)this.PixelData).AbsoluteMinPixelValue, 
					((IndexedPixelData)base.PixelData).AbsoluteMaxPixelValue, "value");

				_maxPixelValue = value;
			}
		}

		/// <summary>
		/// Gets the modality LUT.
		/// </summary>
		public IComposableLut ModalityLut
		{
			get { return this.LutComposer.LutCollection[0]; }
		}

		public void SetVoiLut(string name)
		{
			
		}

		/// <summary>
		/// Gets the VOI LUT.
		/// </summary>
		public IVoiLut VoiLut
		{
			get { return this.LutComposer.LutCollection[1] as IVoiLut; }
		}

		public void SetPresentationLut(string name)
		{
			
		}

		/// <summary>
		/// Gets the presentation LUT.
		/// </summary>
		public IPresentationLut PresentationLut
		{
			get { return this.LutComposer.LutCollection[2] as IPresentationLut; }
		}

		/// <summary>
		/// The output of the LUT pipeline.
		/// </summary>
		/// <remarks>
		/// Each entry in the <see cref="OutputLut"/> array is 32-bit ARGB value.
		/// When an <see cref="IRenderer"/> renders an <see cref="IndexedImageGraphic"/>, it should
		/// use <see cref="OutputLut"/> to determine the ARGB value to display for a given pixel value.
		/// </remarks>
		public override int[] OutputLut
		{
			get { return this.LutComposer.OutputLut; }
		}

		#region IVOILUTLinearProvider Members

		/// <summary>
		/// Gets the linear VOI LUT.
		/// </summary>
		/// <value>The linear VOI LUT or <b>null</b> if the VOI LUT
		/// is not linear.</value>
		public virtual IVoiLutLinear VoiLutLinear
		{
			get { return this.VoiLut as IVoiLutLinear; }
		}

		#endregion

		#endregion

		#region Private properties

		/// <summary>
		/// Gets the <see cref="LutComposer"/>.
		/// </summary>
		private LutComposer LutComposer
		{
			get
			{
				if (_lutComposer == null)
					_lutComposer = new LutComposer();

				return _lutComposer;
			}
		}

		private LutFactory LutFactory
		{
			get
			{
				if (_lutFactory == null)
					_lutFactory = LutFactory.NewInstance;

				return _lutFactory;
			}
		}

		#endregion

		#region Disposal

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_lutFactory != null)
					_lutFactory.Dispose();

				if (_lutComposer != null)
					_lutComposer.Dispose();
			}
		}

		#endregion

		//#region Static methods

		//public static IStatefulVoiLutLinear NewVoiLutLinear(GrayscaleImageGraphic fromGraphic, IVoiLutLinearState state)
		//{
		//    return new StatefulVoiLutLinear(state, fromGraphic.ModalityLUT.MinOutputValue, fromGraphic.ModalityLUT.MaxOutputValue);
		//}

		//public static IStatefulVoiLutLinear NewVoiLutLinear(GrayscaleImageGraphic fromGraphic)
		//{
		//    return new StatefulVoiLutLinear(fromGraphic.ModalityLUT.MinOutputValue, fromGraphic.ModalityLUT.MaxOutputValue);
		//}

		//#endregion

		#region Private methods

		private void InstallGrayscaleLuts(
			double rescaleSlope, 
			double rescaleIntercept,
			bool inverted)
		{
			ModalityLutLinear modalityLut = this.LutFactory.GetModalityLutLinear(
				this.BitsStored,
				this.IsSigned,
				rescaleSlope,
				rescaleIntercept);

			this.LutComposer.LutCollection.Add(modalityLut);

			VoiLutLinear voiLut = new VoiLutLinear(modalityLut.MinOutputValue, modalityLut.MaxOutputValue);
			this.LutComposer.LutCollection.Add(voiLut);

			PresentationLut presentationLut = this.LutFactory.GetPresentationLut(
				voiLut.MinOutputValue,
				voiLut.MaxOutputValue,
				inverted);

			this.LutComposer.LutCollection.Add(presentationLut);
		}

		#endregion
	}
}
