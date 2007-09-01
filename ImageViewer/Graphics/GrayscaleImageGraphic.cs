using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A grayscale <see cref="IndexedImageGraphic"/>.
	/// </summary>
	public class GrayscaleImageGraphic : IndexedImageGraphic, IModalityLutProvider, IVoiLutProvider, IPresentationLutProvider
	{
		private enum Luts
		{ 
			Modality = 1,
			Voi = 2,
			Presentation = 3
		}

		#region Private fields

		private LutComposer _lutComposer;
		private LutFactory _lutFactory;

		private bool _inverted;
		private double _rescaleSlope;
		private double _rescaleIntercept;

		private IVoiLutManager _voiLutManager;
		private IPresentationLutManager _presentationLutManager;

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
			Initialize(false, 1, 0);
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
			Initialize(false, rescaleSlope, rescaleIntercept);
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
			_inverted = inverted;
			_rescaleSlope = rescaleSlope <= double.Epsilon ? 1 : rescaleSlope;
			_rescaleIntercept = rescaleIntercept;
		}

		#endregion

		#region Public properties

		#region IVoiLutProvider Members

		public IVoiLutManager VoiLutManager
		{
			get 
			{
				if (_voiLutManager == null)
					_voiLutManager = new VoiLutManager(this);

				return _voiLutManager;
			}
		}

		#endregion

		#region IPresentationLutProvider Members

		public IPresentationLutManager PresentationLutManager
		{
			get
			{
				if (_presentationLutManager == null)
					_presentationLutManager = new PresentationLutManager(this);

				return _presentationLutManager;
			}
		}

		#endregion
		
		/// <summary>
		/// Gets the modality LUT.
		/// </summary>
		public IModalityLut ModalityLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Modality);
				return this.LutComposer.LutCollection[0] as IModalityLut; 
			}
		}

		/// <summary>
		/// Gets the VOI LUT.
		/// </summary>
		public ILut VoiLut
		{
			get
			{
				InitializeNecessaryLuts(Luts.Voi);
				return this.LutComposer.LutCollection[1]; 
			}
		}

		/// <summary>
		/// Gets the presentation LUT.
		/// </summary>
		public IPresentationLut PresentationLut
		{
			get 
			{
				InitializeNecessaryLuts(Luts.Presentation);
				return this.LutComposer.LutCollection[2] as IPresentationLut; 
			}
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
			get 
			{
				InitializeNecessaryLuts(Luts.Presentation); 
				return this.LutComposer.OutputLut; 
			}
		}

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

		#region Private methods

		private void InitializeNecessaryLuts(Luts luts)
		{
			if (luts >= Luts.Modality && LutComposer.LutCollection.Count == 0)
			{
				IModalityLut modalityLut =
					this.LutFactory.GetModalityLutLinear(this.BitsStored, this.IsSigned, _rescaleSlope, _rescaleIntercept);
			
				this.LutComposer.LutCollection.Add(modalityLut);
			}

			if (luts >= Luts.Voi && LutComposer.LutCollection.Count == 1)
			{
				ILut lut = InitialVoiLutProvider.Instance.GetLut(this.ParentPresentationImage);
				
				if (lut == null)
					lut = new MinMaxPixelCalculatedLinearLut(this.PixelData, this.ModalityLut);

				InstallVoiLut(lut);
			}

			if (luts >= Luts.Presentation && LutComposer.LutCollection.Count == 2)
			{
				InstallPresentationLut(GrayscalePresentationLutFactory.FactoryName);
			}
		}

		#endregion

		#region Internal Properties / Methods

		internal IEnumerable<PresentationLutDescriptor> AvailablePresentationLuts
		{
			get
			{
				return this.LutFactory.AvailablePresentationLuts;
			}
		}

		internal void InstallVoiLut(ILut voiLut)
		{
			Platform.CheckForNullReference(voiLut, "voiLut");

			InitializeNecessaryLuts(Luts.Modality);

			if (this.LutComposer.LutCollection.Count == 1)
			{
				this.LutComposer.LutCollection.Add(voiLut);
			}
			else
			{
				this.LutComposer.LutCollection[1] = voiLut;
			}
		}

		internal void InstallPresentationLut(string name)
		{
			IPresentationLut lut = this.LutFactory.GetPresentationLut(name);
			lut.Invert = _inverted;
			InstallPresentationLut(lut);
		}

		/// <summary>
		/// Only to be used directly (outside of this class) for restoring Luts from mementos.
		/// </summary>
		/// <param name="installLut"></param>
		internal void InstallPresentationLut(IPresentationLut installLut)
		{
			InitializeNecessaryLuts(Luts.Voi);

			if (this.LutComposer.LutCollection.Count == 2)
			{
				this.LutComposer.LutCollection.Add(installLut);
			}
			else
			{
				this.LutComposer.LutCollection[2] = installLut;
			}
		}

		#endregion
	}
}
