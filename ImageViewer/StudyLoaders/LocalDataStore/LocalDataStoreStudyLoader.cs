using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyLoaderExtensionPoint))]
    public class LocalDataStoreStudyLoader : StudyLoader
    {
        public LocalDataStoreStudyLoader()
        {

        }

        public override string Name
        {
            get
            {
                return "My DataStore";
            }
        }

        public override void LoadStudy(string studyUID)
        {
			bool atLeastOneImageFailedToLoad = false;

			try
            {

                IStudy study = DataAbstractionLayer.GetIDataStore().GetStudy(new Uid(studyUID));
                IEnumerable<ISopInstance> listOfSops = study.GetSopInstances();
				int imagesLoaded = 0;

                foreach (ISopInstance sop in listOfSops)
                {
                    // TODO: don't just skip non-image sop instances
                    ImageSopInstance imageObject = sop as ImageSopInstance;
                    if (null == imageObject)
                        continue;

                    // skip non local objects
                    if (!imageObject.LocationUri.IsFile)
                        continue;

                    LocalDataStoreImageSop localImage = new LocalDataStoreImageSop(imageObject);                  

                    try
                    {
						ImageViewerComponent.StudyManager.StudyTree.AddImage(localImage);
						imagesLoaded++;
                    }
                    catch (ImageValidationException e)
                    {
						atLeastOneImageFailedToLoad = true;

                        Platform.Log(e, LogLevel.Warn);
                    }
                }

				bool studyCouldNotBeLoaded = (imagesLoaded == 0);

				if (atLeastOneImageFailedToLoad || studyCouldNotBeLoaded)
				{
					OpenStudyException e = new OpenStudyException("An error occurred while opening the study");
					e.AtLeastOneImageFailedToLoad = atLeastOneImageFailedToLoad;
					e.StudyCouldNotBeLoaded = studyCouldNotBeLoaded;
					throw e;
				}
            }
            catch (Exception e)
            {
				// We probably have a database error.  Make note of the exception as an inner exception.
				OpenStudyException ex = new OpenStudyException("An error occurred while opening the study", e);
				ex.StudyCouldNotBeLoaded = true;
				ex.AtLeastOneImageFailedToLoad = true;

				throw ex;
            }

            base.LoadStudy(studyUID);
        }

        //private void MapImageObjectToLocalImageObject(ImageSopInstance imageObject, IDicomPropertySettable localImage)
        //{
        //    ClearCanvas.Dicom.DataStore.Study study = imageObject.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Study;
        //    ClearCanvas.Dicom.DataStore.Series series = imageObject.GetParentSeries().GetParentStudy() as ClearCanvas.Dicom.DataStore.Series;

        //    // string properties
        //    localImage.SetStringProperty("PatientId", study.PatientId);
        //    localImage.SetStringProperty("StudyInstanceUid", study.StudyInstanceUid);
        //    localImage.SetStringProperty("SeriesInstanceUid", series.SeriesInstanceUid);
        //    localImage.SetStringProperty("TransferSyntaxUid", imageObject.TransferSyntaxUid);
        //    localImage.SetStringProperty("SopInstanceUid", imageObject.SopInstanceUid);

        //    // integer properties
        //    localImage.SetInt32Property("SamplesPerPixel", imageObject.SamplesPerPixel);
        //    localImage.SetInt32Property("Rows", imageObject.Rows);
        //}
    }
}
