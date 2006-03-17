namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IDicomPropertySettable
    {
        void SetStringProperty(String propertyName, String value);
        void SetInt32Property(String propertyName, Int32 value);
        void SetUInt32Property(String propertyName, UInt32 value);
        void SetDoubleProperty(String propertyName, Double value);
    }
}
