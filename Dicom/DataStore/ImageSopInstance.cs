#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Dicom.DataStore
{
    public class ImageSopInstance : SopInstance
	{
		#region Private Fields

		private int _bitsAllocated;
    	private int _bitsStored;
    	private int _highBit;
		private int _pixelRepresentation;
		private int _samplesPerPixel;
		private int _planarConfiguration;
    	private PhotometricInterpretation _photometricInterpretation;
    	private int _rows;
    	private int _columns;
    	private PixelSpacing _pixelSpacing;
    	private PixelAspectRatio _pixelAspectRatio;
    	private ImagePositionPatient _imagePositionPatient;
    	private ImageOrientationPatient _imageOrientationPatient;
    	private double _rescaleSlope;
    	private double _rescaleIntercept;
    	private int _numberOfFrames;
    	private readonly IList _windowValues;

		#endregion

		protected internal ImageSopInstance()
        {
            _windowValues = new ArrayList();
		}

		#region NHibernate Persistent Properties

		public virtual int BitsAllocated
    	{
    		get { return _bitsAllocated; }
			set { SetValueTypeMember(ref _bitsAllocated, value); }
    	}

		public virtual int BitsStored
    	{
    		get { return _bitsStored; }
			set { SetValueTypeMember(ref _bitsStored, value); }
    	}

		public virtual int HighBit
    	{
    		get { return _highBit; }
			set { SetValueTypeMember(ref _highBit, value); }
    	}

		public virtual int PixelRepresentation
    	{
    		get { return _pixelRepresentation; }
			set { SetValueTypeMember(ref _pixelRepresentation, value); }
    	}

		public virtual int SamplesPerPixel
        {
            get { return _samplesPerPixel; }
			set { SetValueTypeMember(ref _samplesPerPixel, value); }
        }

		public virtual int PlanarConfiguration
    	{
    		get { return _planarConfiguration; }
			set { SetValueTypeMember(ref _planarConfiguration, value); }
    	}

    	public virtual PhotometricInterpretation PhotometricInterpretation
    	{
    		get { return _photometricInterpretation; }
			set { SetValueTypeMember(ref _photometricInterpretation, value); }
    	}

    	public virtual int Rows
    	{
    		get { return _rows; }
			set { SetValueTypeMember(ref _rows, value); }
    	}

    	public virtual int Columns
    	{
    		get { return _columns; }
			set { SetValueTypeMember(ref _columns, value); }
    	}

    	public virtual PixelSpacing PixelSpacing
    	{
    		get { return _pixelSpacing; }
			set { SetClassMember(ref _pixelSpacing, value); }
    	}

    	public virtual PixelAspectRatio PixelAspectRatio
    	{
    		get { return _pixelAspectRatio; }
			set { SetClassMember(ref _pixelAspectRatio, value); }
    	}

		public virtual ImageOrientationPatient ImageOrientationPatient
		{
			get { return _imageOrientationPatient; }
			set { SetClassMember(ref _imageOrientationPatient, value); }
		}

		public virtual ImagePositionPatient ImagePositionPatient
		{
			get { return _imagePositionPatient; }
			set { SetClassMember(ref _imagePositionPatient, value); }
		}

		public virtual double RescaleSlope
        {
            get { return _rescaleSlope; }
			set { SetValueTypeMember(ref _rescaleSlope, value); }
        }

    	public virtual double RescaleIntercept
        {
            get { return _rescaleIntercept; }
			set { SetValueTypeMember(ref _rescaleIntercept, value); }
        }

    	public virtual int NumberOfFrames
    	{
			get { return _numberOfFrames; }
			set { SetValueTypeMember(ref _numberOfFrames, value); }
    	}

    	public virtual IList WindowValues
        {
            get { return _windowValues; }
		}

		#endregion

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		#region Helper Methods

		protected internal override void Update(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
		{
			base.Update(metaInfo, sopInstanceDataset);

			DicomAttribute attribute = sopInstanceDataset[DicomTags.BitsAllocated];
			BitsAllocated = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.BitsStored];
			BitsStored = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.HighBit];
			HighBit = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.PixelRepresentation];
			PixelRepresentation = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.SamplesPerPixel];
			SamplesPerPixel = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.PlanarConfiguration];
			PlanarConfiguration = attribute.GetUInt16(0, 0);
			
			attribute = sopInstanceDataset[DicomTags.PhotometricInterpretation];
			PhotometricInterpretation = PhotometricInterpretationHelper.FromString(attribute.ToString());

			attribute = sopInstanceDataset[DicomTags.Rows];
			Rows = attribute.GetUInt16(0, 0);

			attribute = sopInstanceDataset[DicomTags.Columns];
			Columns = attribute.GetUInt16(0, 0);

			double pixelSpacingX = 0, pixelSpacingY = 0;
			attribute = sopInstanceDataset[DicomTags.PixelSpacing];
			if (attribute.Count == 2)
			{
				if (!attribute.TryGetFloat64(0, out pixelSpacingX) || !attribute.TryGetFloat64(1, out pixelSpacingY))
					pixelSpacingX = pixelSpacingY = 0;
			}
			PixelSpacing = new PixelSpacing(pixelSpacingX, pixelSpacingY);

			double pixelAspectRatioX = 0, pixelAspectRatioY = 0;
			attribute = sopInstanceDataset[DicomTags.PixelAspectRatio];
			if (attribute.Count == 2)
			{
				if (!attribute.TryGetFloat64(0, out pixelAspectRatioX) || !attribute.TryGetFloat64(1, out pixelAspectRatioY))
					pixelAspectRatioX = pixelAspectRatioY = 0;
			}
			PixelAspectRatio = new PixelAspectRatio(pixelAspectRatioX, pixelAspectRatioY);

			double imagePositionPatientX = 0, imagePositionPatientY = 0, imagePositionPatientZ = 0;
			attribute = sopInstanceDataset[DicomTags.ImagePositionPatient];
			if (attribute.Count == 3)
			{
				if (!attribute.TryGetFloat64(0, out imagePositionPatientX) || !attribute.TryGetFloat64(1, out imagePositionPatientY) ||
					!attribute.TryGetFloat64(2, out imagePositionPatientZ))
					imagePositionPatientX = imagePositionPatientY = imagePositionPatientZ = 0;
			}
			ImagePositionPatient = new ImagePositionPatient(imagePositionPatientX, imagePositionPatientY, imagePositionPatientZ);

			double imageOrientationPatientRowX = 0, imageOrientationPatientRowY = 0, imageOrientationPatientRowZ = 0;
			double imageOrientationPatientColumnX = 0, imageOrientationPatientColumnY = 0, imageOrientationPatientColumnZ = 0;
			attribute = sopInstanceDataset[DicomTags.ImageOrientationPatient];
			if (attribute.Count == 6)
			{
				if (!attribute.TryGetFloat64(0, out imageOrientationPatientRowX) || !attribute.TryGetFloat64(1, out imageOrientationPatientRowY) ||
					!attribute.TryGetFloat64(2, out imageOrientationPatientRowZ) || !attribute.TryGetFloat64(3, out imageOrientationPatientColumnX) ||
					!attribute.TryGetFloat64(4, out imageOrientationPatientColumnY) || !attribute.TryGetFloat64(5, out imageOrientationPatientColumnZ))
				{
					imageOrientationPatientRowX = imageOrientationPatientRowY = imageOrientationPatientRowZ =
					                                                            imageOrientationPatientColumnX =
					                                                            imageOrientationPatientColumnY =
					                                                            imageOrientationPatientColumnZ = 0;
				}
			}
			ImageOrientationPatient =
				new ImageOrientationPatient(imageOrientationPatientRowX, imageOrientationPatientRowY, imageOrientationPatientRowZ,
				                            imageOrientationPatientColumnX, imageOrientationPatientColumnY,
				                            imageOrientationPatientColumnZ);

			double doubleValue;
			attribute = sopInstanceDataset[DicomTags.RescaleSlope];
			attribute.TryGetFloat64(0, out doubleValue);
			RescaleSlope = doubleValue;

			attribute = sopInstanceDataset[DicomTags.RescaleIntercept];
			attribute.TryGetFloat64(0, out doubleValue);
			RescaleIntercept = doubleValue;

			attribute = sopInstanceDataset[DicomTags.NumberOfFrames];
			_numberOfFrames = attribute.GetInt32(0, 0);

			List<Window> windowValues = new List<Window>();

			attribute = sopInstanceDataset[DicomTags.WindowWidth];
			if (attribute.Count > 0 && !attribute.IsNull && !attribute.IsEmpty)
			{
				DicomAttribute attribute2 = sopInstanceDataset[DicomTags.WindowCenter];
				if (attribute.Count == attribute2.Count && !attribute2.IsNull && !attribute2.IsEmpty)
				{
					for (int i = 0; i < attribute.Count; ++i)
					{
						double doubleValue1, doubleValue2;
						if (attribute.TryGetFloat64(i, out doubleValue1) && attribute2.TryGetFloat64(i, out doubleValue2))
							windowValues.Add(new Window(doubleValue1, doubleValue2));
					}
				}
			}

			if (windowValues.Count != WindowValues.Count)
			{
				WindowValues.Clear();
				foreach (Window window in windowValues)
					WindowValues.Add(window);

				OnChanged();
			}
			else
			{
				int i = 0;
				foreach (Window window in WindowValues)
				{
					Window newValue = windowValues[i++];
					if (!window.Equals(newValue))
					{
						window.Width = newValue.Width;
						window.Center = newValue.Center;
						OnChanged();
					}
				}
			}
		}

    	#endregion
	}
}
