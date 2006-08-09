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

        public virtual short SamplesPerPixel
        {
            get { return _samplesPerPixel; }
            set { _samplesPerPixel = value; }
        }

        public virtual short BitsStored
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

        public virtual short PlanarConfiguration
        {
            get { return _planarConfiguration; }
            set { _planarConfiguration = value; }
        }

        public virtual double RescaleIntercept
        {
            get { return _rescaleIntercept; }
            set { _rescaleIntercept = value; }
        }

        public virtual short PixelRepresentation
        {
            get { return _pixelRepresentation; }
            set { _pixelRepresentation = value; }
        }

        public virtual short BitsAllocated
        {
            get { return _bitsAllocated; }
            set { _bitsAllocated = value; }
        }

        public virtual short HighBit
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

            if (this.Oid != 0 && sop.Oid != 0)
            {
                if (this.Oid != sop.Oid)
                    return false;
            }

            if (this.SopInstanceUid != sop.SopInstanceUid)
                return false;

            return true;
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
        private short _samplesPerPixel;
        private short _bitsStored;
        private double _rescaleSlope;
        private PixelSpacing _pixelSpacing;
        private int _rows;
        private int _columns;
        private PhotometricInterpretation _photometricInterpretation;
        private short _planarConfiguration;
        private double _rescaleIntercept;
        private short _pixelRepresentation;
        private short _bitsAllocated;
        private short _highBit;
        private IList _windowValues;
        #endregion
    }
}
