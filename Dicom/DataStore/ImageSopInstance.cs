using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections;


namespace ClearCanvas.Dicom.DataStore
{
    public class ImageSopInstance : SopInstance
    {
        public ImageSopInstance()
        {
            _windowValues = new ArrayList();
        }

        public virtual int SamplesPerPixel
        {
            get { return _samplesPerPixel; }
            set { _samplesPerPixel = value; }
        }

        public virtual int BitsStored
        {
            get { return _bitsStored; }
            set { _bitsStored = value; }
        }

        public virtual double RescaleSlope
        {
            get { return _rescaleSlope; }
            set { _rescaleSlope = value; }
        }

        public virtual int Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        public virtual int Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public virtual int PlanarConfiguration
        {
            get { return _planarConfiguration; }
            set { _planarConfiguration = value; }
        }

        public virtual double RescaleIntercept
        {
            get { return _rescaleIntercept; }
            set { _rescaleIntercept = value; }
        }

        public virtual int PixelRepresentation
        {
            get { return _pixelRepresentation; }
            set { _pixelRepresentation = value; }
        }

        public virtual int BitsAllocated
        {
            get { return _bitsAllocated; }
            set { _bitsAllocated = value; }
        }

        public virtual int HighBit
        {
            get { return _highBit; }
            set { _highBit = value; }
        }

        public virtual PhotometricInterpretation PhotometricInterpretation
        {
            get { return _photometricInterpretation; }
            set { _photometricInterpretation = value; }
        }

        public virtual PixelSpacing PixelSpacing
        {
            get { return _pixelSpacing; }
            set { _pixelSpacing = value; }
        }

        public virtual PixelAspectRatio PixelAspectRatio
        {
            get { return _pixelAspectRatio; }
            set { _pixelAspectRatio = value; }
        }

        public virtual IList WindowValues
        {
            get { return _windowValues; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            ImageSopInstance sop = obj as ImageSopInstance;
            if (null == sop)
                return false; // null or not a sop

            return Equals(sop);
        }

        public bool Equals(ImageSopInstance sop)
        {
            // if the base class Equals returns true AND
            // my own values are equal, then we should return true
            bool isInstanceUidIdentical = this.SopInstanceUid == sop.SopInstanceUid;
            bool isLocationUriIdentical = this.LocationUri != null && sop.LocationUri != null &&
                this.LocationUri.Equals(sop.LocationUri);

            return base.Equals(sop) && isInstanceUidIdentical && isLocationUriIdentical;
        }

        public override int GetHashCode()
        {
            int accumulator = 0;
            foreach (char character in this.SopInstanceUid)
            {
                if ('.' != character)
                    accumulator += Convert.ToInt32(character);
                else
                    accumulator -= 19;
            }
            return accumulator;
        }

        #region Private members
        private int _samplesPerPixel;
        private int _bitsStored;
        private double _rescaleSlope;
        private PixelSpacing _pixelSpacing;
        private PixelAspectRatio _pixelAspectRatio;
        private int _rows;
        private int _columns;
        private PhotometricInterpretation _photometricInterpretation;
        private int _planarConfiguration;
        private double _rescaleIntercept;
        private int _pixelRepresentation;
        private int _bitsAllocated;
        private int _highBit;
        private IList _windowValues;
        #endregion
    }
}
