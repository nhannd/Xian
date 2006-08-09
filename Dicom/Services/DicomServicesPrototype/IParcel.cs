
namespace ClearCanvas.Dicom.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom;

    public interface IParcel
    {
        int Include(Uid referencedUid);
    }
}
