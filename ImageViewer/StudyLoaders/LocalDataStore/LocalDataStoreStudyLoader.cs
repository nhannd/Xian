using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    [ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyLoaderExtensionPoint))]
    public class LocalDataStoreStudyLoader : IStudyLoader
    {
		private IEnumerator<ISopInstance> _sops;

        public LocalDataStoreStudyLoader()
        {

        }

        public string Name
        {
            get
            {
                return "DICOM_LOCAL";
            }
        }

		public int Start(string studyInstanceUID)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(new Uid(studyInstanceUID));
				_sops = new List<ISopInstance>(study.GetSopInstances()).GetEnumerator();
				_sops.Reset();

				return study.GetNumberOfSopInstances();
			}
		}

		public ImageSop LoadNextImage()
        {
			ImageSopInstance imageObject;

			while (true)
			{
				bool moreImages = _sops.MoveNext();

				if (!moreImages)
					return null;

				imageObject = _sops.Current as ImageSopInstance;
				
				// TODO: don't just skip non-image sop instances
				if (imageObject != null)
				{
					// Skip non-local images
					if (imageObject.LocationUri.IsFile)
						break;
				}
			}

			LocalDataStoreImageSop localImage = new LocalDataStoreImageSop(imageObject);

			return localImage;
        }
    }
}
