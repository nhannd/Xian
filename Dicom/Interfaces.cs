namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Specifies an interface that allows an object to have
    /// DICOM properties set to specific values. An example where
    /// this interface may be used is in the loading of DICOM image
    /// files from disk, but where the header may be stored in a
    /// fast-access database. The database may use this interface
    /// to set all the properties of the loaded image, from the set
    /// of properties stored in the database, 
    /// while knowing nothing about the type of
    /// of the image object, other than that it implements this 
    /// interface.
    /// </summary>
    public interface IDicomPropertySettable
    {
        void SetStringProperty(String propertyName, String value);
        void SetIntProperty(String propertyName, int value);
        void SetUintProperty(String propertyName, uint value);
        void SetDoubleProperty(String propertyName, Double value);
    }
}
