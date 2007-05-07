using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="GrayscaleImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public class StandardGrayscaleImageGraphic : GrayscaleImageGraphic, IAutoVoiLutApplicatorProvider
	{
		/// <summary>
		/// An internal class that provides automatic Voi Lut calculation capabilities.
		/// </summary>
		/// <remarks>
		/// Currently, only very basic functionality exists, in that this class is only capable
		/// of cycling through the linear Voi Luts specified in the Dicom header ( Window Width / Center).
		/// If these Luts don't exist in the header, rudimentary values are chosen (Window Width = 2^bits stored).
		/// Later, support will be added for Luts from the header's Voi Lut Sequence, including Data Luts.
		/// </remarks>
		private class StandardAutoVoiLutApplicator : IAutoVoiLutApplicator
		{
			private StandardGrayscaleImageGraphic _parentImageGraphic;
			private bool _settingLut;
			private int _currentLutIndex;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="parentImageGraphic">the parent image graphic</param>
			public StandardAutoVoiLutApplicator(StandardGrayscaleImageGraphic parentImageGraphic)
			{
				_parentImageGraphic = parentImageGraphic;
				_parentImageGraphic.VoiLUT.LUTChanged += new EventHandler(OnVoiLutChanged);

				_currentLutIndex = -1;
				_settingLut = false;
			}

			private StandardAutoVoiLutApplicator()
			{ 
			}

			/// <summary>
			/// Gets the Window Width/Center values from the Dicom Header of the parent image.
			/// </summary>
			private Window[] HeaderLuts
			{
				get { return _parentImageGraphic.ImageSop.WindowCenterAndWidth; }
			}

			/// <summary>
			/// Executed when the parent image's Voi lut has changed in some way.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnVoiLutChanged(object sender, EventArgs e)
			{
				if (!_settingLut)
					_currentLutIndex = -1;
			}

			/// <summary>
			/// Internally, Luts are applied based on the order in which they appear in the 
			/// Dicom Header.  This method simply applies the Lut at the current index.
			/// </summary>
			private void ApplyLutAtCurrentIndex()
			{
				if (_parentImageGraphic.VoiLUTLinear == null)
					return;

				double windowWidth = double.NaN;
				double windowCenter = double.NaN;

				if (this.HeaderLuts != null)
				{
					windowWidth = this.HeaderLuts[_currentLutIndex].Width;
					windowCenter = this.HeaderLuts[_currentLutIndex].Center;
				}

				//Window Width must be non-zero according to DICOM.
				//Otherwise, we want to do something simple (pick 2^BitsStored).
				if (windowWidth == 0 || double.IsNaN(windowWidth))
					windowWidth = 1 << ((int)_parentImageGraphic.BitsStored);

				//If Window Center is invalid, calculate a value based on the Window Width.
				if (double.IsNaN(windowCenter))
				{
					if (_parentImageGraphic.PixelRepresentation == 0)
						windowCenter = ((int)windowWidth) >> 1;
					else
						windowCenter = 0;
				}

				try
				{
					_settingLut = true;
					_parentImageGraphic.VoiLUTLinear.WindowWidth = windowWidth;
					_parentImageGraphic.VoiLUTLinear.WindowCenter = windowCenter;
				}
				finally
				{
					_settingLut = false;
				}
			}

			#region IAutoVoiLutApplicator Members

			/// <summary>
			/// Applies the next Lut from the header.
			/// </summary>
			/// <remarks>
			/// This method, when called repeatedly, will apply the header Luts in a cyclic 
			/// fashion (0, 1, 2, 0, 1, 2 ...).  If a header lut is not currently applied, 
			/// the first Lut from the header is applied.  If there are no header Luts, a Lut
			/// with Window Width = 2^bits stored is applied.
			/// </remarks>
			public void ApplyNext()
			{
				//wrap the index around in order to cycle through the Luts.
				if (this.HeaderLuts == null || ++_currentLutIndex >= HeaderLuts.Length)
					_currentLutIndex = 0;

				ApplyLutAtCurrentIndex();
			}

			#endregion

			#region IMemorable Members

			/// <summary>
			/// Implements the Memento pattern.  This method creates a memento for the Voi
			/// Lut that is currently applied so that it can be restored by an undo operation.
			/// </summary>
			/// <returns>either an <see cref="AutoVoiLutMemento"/> or a <see cref="WindowLevelMemento"/>
			/// depending on the currently applied Lut.</returns>
			public IMemento CreateMemento()
			{
				if (_currentLutIndex >= 0)
				{
					return new AutoVoiLutMemento(_currentLutIndex);
				}
				else
				{
					WindowLevelMemento returnMemento = new WindowLevelMemento();
					returnMemento.WindowWidth = _parentImageGraphic.VoiLUTLinear.WindowWidth;
					returnMemento.WindowCenter = _parentImageGraphic.VoiLUTLinear.WindowCenter;
					return returnMemento;
				}
			}

			/// <summary>
			/// Implements the memento pattern.  This method changes the parent image's Voi Lut based on the
			/// supplied memento (either an <see cref="AutoVoiLutMemento"/> or a <see cref="WindowLevelMemento"/>).
			/// </summary>
			/// <param name="memento">the memento to use to change the parent image's Voi Lut</param>
			public void SetMemento(IMemento memento)
			{
				Platform.CheckForNullReference(memento, "memento");

				AutoVoiLutMemento autoVoiLutMemento = memento as AutoVoiLutMemento;
				WindowLevelMemento windowLevelMemento = memento as WindowLevelMemento;
				
				if (autoVoiLutMemento != null)
				{
					_currentLutIndex = Math.Min(this.HeaderLuts.Length - 1, autoVoiLutMemento.LutIndex);
					_currentLutIndex = Math.Max(0, autoVoiLutMemento.LutIndex);
					ApplyLutAtCurrentIndex();
				}
				else if (windowLevelMemento != null)
				{
					if (_parentImageGraphic.VoiLUTLinear != null)
					{
						_parentImageGraphic.VoiLUTLinear.WindowWidth = windowLevelMemento.WindowWidth;
						_parentImageGraphic.VoiLUTLinear.WindowCenter = windowLevelMemento.WindowCenter;
					}
				}
				else
				{
					throw new InvalidOperationException(SR.ExceptionMementoMustBeEitherAutoOrWindowLevel);
				}
			}

			#endregion
		}

		private ImageSop _imageSop;
		private StandardAutoVoiLutApplicator _autoVoiLutApplicator;

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardGrayscaleImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardGrayscaleImageGraphic(ImageSop imageSop) : 
			base(
			imageSop.Rows,
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.SamplesPerPixel,
			imageSop.PixelRepresentation,
			imageSop.PlanarConfiguration,
			imageSop.PhotometricInterpretation,
			imageSop.RescaleSlope,
			imageSop.RescaleIntercept,
			null)
		{
			Platform.CheckForNullReference(imageSop, "image");
			_imageSop = imageSop;
			CreateAutoVoiLutApplicator();
			Platform.CheckForNullReference(_autoVoiLutApplicator, "_autoVoiLutApplicator");
			_autoVoiLutApplicator.ApplyNext();
		}

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>
		/// Gets the pixel data from the associated <see cref="ImageSop"/>.
		/// </summary>
		protected override byte[] PixelDataRaw
		{
			get
			{
				return _imageSop.PixelData;
			}
		}

		/// <summary>
		/// Creates the <see cref="AutoVoiLutApplicator"/>.  Derived classes can override this method
		/// to install a different type of <see cref="<see cref="IAutoVoiLutApplicator"/>"/>.
		/// </summary>
		protected virtual void CreateAutoVoiLutApplicator()
		{
			_autoVoiLutApplicator = new StandardAutoVoiLutApplicator(this);
		}

		#region IAutoLutApplicatorProvider Members

		/// <summary>
		/// Returns the associated <see cref="IAutoVoiLutApplicator"/> used to perform automatic application/calculation of Voi Luts.
		/// </summary>
		public IAutoVoiLutApplicator AutoVoiLutApplicator
		{
			get { return _autoVoiLutApplicator; }
		}

		#endregion
	}
}
