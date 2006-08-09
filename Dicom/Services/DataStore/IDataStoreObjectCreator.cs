using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public interface IDataStoreObjectCreator
    {
        ISopInstance NewImageSopInstance();
        void StoreImageSopInstance(ISopInstance sop);
    }
}
